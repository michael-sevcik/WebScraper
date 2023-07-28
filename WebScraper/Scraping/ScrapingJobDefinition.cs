using HtmlAgilityPack;

namespace WebScraper.Scraping;

/// <summary>
/// Parses the links to product pages from product list page document node.
/// </summary>
/// <param name="node">The root document node of the product list page.</param>
/// <returns>The links to product pages that were contained in the <paramref name="node"/>.</returns>
public delegate IReadOnlyCollection<Uri> ParseProductPageLinksDelegate(HtmlNode node);

/// <summary>
/// Parses the link to the next page of the product list.
/// </summary>
/// <param name="node">The root document node of the product list page.</param>
/// <returns>The URL address to the next page of the product list.</returns>
public delegate string? ParseNextPageLinkDelegate(HtmlNode node);

/// <summary>
/// Represents an encapsulation of information describing a scraping of a particular product list.
/// </summary>
/// <param name="FirstProductListPageUri">The link to the first page of a product list.</param> // TODO: should be always absolute.
/// <param name="ProductPageLinksParser">A delegate that parses product page links from a product list page.</param>
/// <param name="NextPageLinkParser">A delegate that parses next product list page URL from the current page.</param>
public record ScrapingJobDefinition(
    Uri FirstProductListPageUri,
    ParseProductPageLinksDelegate ProductPageLinksParser,
    ParseNextPageLinkDelegate NextPageLinkParser); // TODO: Add product page parser
