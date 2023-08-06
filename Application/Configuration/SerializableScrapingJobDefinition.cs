using Application.Parsing;
using ProductListCrawler;
using WebScraper.Scraping;

namespace Application.Configuration;

/// <summary>
/// Serializable class that represents a definition for scraping auctions using <see cref="ProductPageProcessor"/> and <see cref="ProductListProcessor"/>
/// </summary>
/// <param name="FirstProductListPageUris">
/// The links to the first page of a product lists for which <paramref name="ProductListProcessor"/> and <paramref name="ProductPageProcessor"/> works.
/// <b>The links should be absolute.</b>
/// </param>
/// <param name="ListProcessorConfiguration">
/// The <see cref="ProductListProcessorConfiguration"/> with configuration for processing <paramref name="FirstProductListPageUris"/>.
/// </param>
/// <param name="PageProcessorConfiguration">
/// The <see cref="ProductPageProcessorConfiguration"/> configured for processing the individual product pages.
/// </param>
public record SerializableScrapingJobDefinition(
    List<Uri> FirstProductListPageUris,
    ProductListProcessorConfiguration ListProcessorConfiguration,
    ProductPageProcessorConfiguration PageProcessorConfiguration);