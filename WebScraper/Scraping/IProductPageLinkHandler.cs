using System.Threading.Tasks.Dataflow;
using WebScraper.Persistence.AuctionRecord;

namespace WebScraper.Scraping;

/// <summary>
/// Represents a class for handling the product (auction) page links.
/// </summary>
public interface IProductPageLinkHandler
{
    /// <summary>
    /// Asynchronously handles the <paramref name="links"/>
    /// - downloads their referred content, parses it and passes the records to an instance of <see cref="IAuctionRecordManager"/>.
    /// </summary>
    /// <param name="links">The links to handle.</param>
    /// <param name="targetBlock">The target block to which the parsed product pages should be passed.</param>
    /// <param name="cancellationToken">A cancellation token to signal that the handling should stop.</param>
    /// <returns>The task object that represents the asynchronous operation.</returns>
    Task HandleLinksAsync(
        IEnumerable<Uri> links,
        ITargetBlock<ProductPageParsingResult> targetBlock,
        CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously gets the auction record from the specified URI.
    /// </summary>
    /// <param name="link">The link from which the new data should be scraped.</param>
    /// <returns>
    /// The task object representing the asynchronous operation,
    /// the result property on the task will return an instance of <see cref="ParsedProductPage"/> on success.
    /// </returns>
    Task<ParsedProductPage?> GetAuctionRecord(Uri link);
}