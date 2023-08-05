using HtmlAgilityPack;
using ProductListCrawler;
using WebScraper.AuctionRecord;

namespace WebScraper.Scraping;

/// <summary>
/// Class for processing product pages.
/// </summary>
public interface IProductPageProcessor
{
    /// <summary>
    /// Parses the product pages and extracts all the information into a <see cref="BaseAuctionRecord"/> as an asynchronous operation.
    /// </summary>
    /// <param name="htmlDocument">The product page to be parsed.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The Result property on the task object returns parsed information in an instance of <see cref="BaseAuctionRecord"/>.
    /// </returns>
    /// <exception cref="ParseException">This exception is thrown when an error occurred during parsing of <paramref name="htmlDocument"/>.</exception>
    Task<BaseAuctionRecord> ParseProductPageAsync(HtmlDocument htmlDocument);
}
