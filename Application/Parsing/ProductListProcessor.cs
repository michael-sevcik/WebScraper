﻿using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using ProductListCrawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Parsing;

/// <inheritdoc/>
public sealed class ProductListProcessor : IProductListProcessor
{
    private readonly ProductListProcessorConfiguration _processorConfiguration;

    /// <summary>
    /// Initializes a new instance of <see cref="ProductListProcessor"/> with the behavior specified in <paramref name="processorConfiguration"/>.
    /// </summary>
    /// <param name="processorConfiguration">The configuration for specifying the behavior of the processer.</param>
    public ProductListProcessor(ProductListProcessorConfiguration processorConfiguration)
    {
        this._processorConfiguration = processorConfiguration;
    }

    /// <summary>
    /// Processes the <paramref name="productListPage"/> and extracts the relevant information from it.
    /// </summary>
    /// <param name="productListPage">The <c>HTML product list page</c> to be processed.</param>
    /// <returns>An instance of <see cref="ProcessedProductList"/> that encapsulates the processed data.</returns>
    public ProcessedProductList Process(HtmlDocument productListPage)
    {
        var documentNode = productListPage.DocumentNode;
        ProcessedProductList result = new();

        // process the product list page
        result.nextPage = ParseNextPage(
            documentNode,
            _processorConfiguration.TypeOfNextPageSelection,
            _processorConfiguration.NextPageCSSSelector);

        result.productPageUris = ParseLinks(documentNode);

        return result;
    }
    
    /// <inheritdoc/>
    public async Task<ProcessedProductList> ProcessAsync(HtmlDocument productListPage)
        => await Task.Run(() => this.Process(productListPage));

    public static Uri ParseUriLink(HtmlNode node)
    {
        var linkNode = node.SelectSingleNode(".//a[@href]");
        string link = linkNode.GetAttributeValue("href", string.Empty);

        return new Uri(link);
    }

    public static string? ParseNextPage(HtmlNode node, NextPageSelectionType pageSelectionType, string nextPageCSSSelector)
    {
        string? nextPage = null;

        // try to find the next page element - either button or the current page number
        var selectedPageElement = node.QuerySelector(nextPageCSSSelector);

        // if there is not a next page element 
        if (selectedPageElement is null)
        {
            return null;
        }

        // According to the pageSelectionType select the link to the next page.
        switch (pageSelectionType)
        {
            case NextPageSelectionType.button:
                nextPage = ParseLink(selectedPageElement); // TODO: check
                break;
            case NextPageSelectionType.nextElementFromCurrenctlySelected:
                var nextPageNode = selectedPageElement.NextSiblingElement();  // TODO: Works, but it would deserve a better solution. // TODO: Crashes on only one page
                if (nextPageNode is null)
                {
                    return null;
                }

                nextPage = ParseLink(nextPageNode);
                break;
            default:
                throw new NotSupportedException();
        }

        return nextPage is null && nextPage == string.Empty? null : nextPage;
    }

    public static string ParseLink(HtmlNode node)
        => node.GetAttributeValue("href", string.Empty);

    private List<Uri> ParseLinks(HtmlNode node)
    {
        var productRecords = node.QuerySelectorAll(_processorConfiguration.ProductsCssSelector);
        var links = from productRecord in productRecords
                    select ParseUriLink(productRecord);
        return links.ToList();
    }
}