using HtmlAgilityPack;
using ProductListCrawler;

namespace WebScraper.Scraping;

/// <summary>
/// Class for processing product pages.
/// </summary>
public interface IProductPageProcessor
{
    /// <summary>
    /// Parses the product pages and extracts all the information into a <see cref="ParsedProductPage"/> as an asynchronous operation.
    /// </summary>
    /// <param name="htmlDocument">The product page to be parsed.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The Result property on the task object returns parsed information in an instance of <see cref="ParsedProductPage"/>.
    /// </returns>
    /// <exception cref="ParseException">An error occurred during parsing of <paramref name="htmlDocument"/>.</exception>
    Task<ParsedProductPage> ParseProductPageAsync(HtmlDocument htmlDocument);
}
