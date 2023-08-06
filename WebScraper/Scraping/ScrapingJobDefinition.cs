using ProductListCrawler;

namespace WebScraper.Scraping;

/// <summary>
/// Represents an encapsulation of information describing a scraping of a particular product list.
/// </summary>
/// <param name="FirstProductListPageUris">
/// The links to the first page of a product lists for which <paramref name="ProductListProcessor"/> and <paramref name="ProductPageProcessor"/> works.
/// <b>The links should be absolute.</b>
/// </param>
/// <param name="ProductListProcessor">The <see cref="IProductListProcessor"/> responsible for processing the product list.</param>
/// <param name="ProductPageProcessor">The <see cref="IProductPageProcessor"/> responsible for processing individual product pages.</param>
public record ScrapingJobDefinition(
    IReadOnlyCollection<Uri> FirstProductListPageUris,
    IProductListProcessor ProductListProcessor,
    IProductPageProcessor ProductPageProcessor);
