﻿using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using ProductListCrawler;

namespace WebScraper;

internal class AuktivaProductListProcessor : IProductListProcessor
{
    public ProcessedProductList Process(HtmlDocument productListPage)
    {
        var elements = productListPage.QuerySelectorAll("tbody .name");

        var linkParser = (HtmlNode node ) =>
        {
            var linkNode = node.SelectSingleNode(".//a[@href]");
            string link = linkNode.GetAttributeValue("href", string.Empty);

            return new Uri(link); // TODO: CHECK what the Uri does and whether it's used in a correct way.
        };

        var nextPageParser = (HtmlNode node ) =>
        {
            var selectedPageNumber = node.QuerySelector(".counter a.selected");
            var nextPage = selectedPageNumber.NextSiblingElement(); // TODO: Works, but it would deserve a better solution.
            var hrefValue = nextPage.GetAttributeValue("href", string.Empty);
            return hrefValue == string.Empty ? null : hrefValue;
        };
        List<Uri> productPageUris = new();

        productPageUris.AddRange(elements.Select(linkParser));

        ProcessedProductList result;
        result.productPageUris = productPageUris;
        result.nextPage = nextPageParser(productListPage.DocumentNode);

        return result;
    }

    public Task<ProcessedProductList> ProcessAsync(HtmlDocument productListPage)
    => Task.Run(() => this.Process(productListPage));
}