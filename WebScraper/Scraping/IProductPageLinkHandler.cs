using WebScraper.Persistence.AuctionRecord;

namespace WebScraper.Scraping;

/// <summary>
/// Represents a class for handling the product (auction) page links.
/// </summary>
public interface IProductPageLinkHandler
{
    /// <summary>
    /// Asynchronously handles the <paramref name="links"/>
    /// - downloads their referred content, parses it and passes the their records to an instance of <see cref="IAuctionRecordManager"/>.
    /// </summary>
    /// <param name="links">The links to handle.</param>
    /// <param name="cancellationToken">A cancellation token to signal that the handling should stop.</param>
    /// <returns>The task object that represents the asynchronous operation.</returns>
    Task HandleLinksAsync(IEnumerable<Uri> links, CancellationToken cancellationToken);

    /// <summary>
    /// Updates the data of an auction record that is specified by the <paramref name="id"/>.
    /// </summary>
    /// <param name="link">The link from which the new data should be scraped.</param>
    /// <param name="id">The <see cref="BaseAuctionRecord.Id"/> of the updated record.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    Task UpdateAuctionRecordAsync(Uri link, Guid id);
}