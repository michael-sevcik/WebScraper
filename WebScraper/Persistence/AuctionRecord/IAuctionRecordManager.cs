using System;
using WebScraper.Scraping;

namespace WebScraper.Persistence.AuctionRecord;

/// <summary>
/// Represents a class for managing auction records.
/// </summary>
public interface IAuctionRecordManager
{
    /// <summary>
    /// Handles the <paramref name="parsedProductPage"/> - creates new <see cref="BaseAuctionRecord"/> if it is new, or sends a notification if it was readded.
    /// </summary>
    /// <param name="parsedProductPage">The parsed auction page to be handled.</param>
    /// <param name="sourceUri">The source URI of the parsed auction page.</param>
    /// <param name="productPageProcessor">The product page processor that was used to create the <paramref name="parsedProductPage"/>.</param>
    /// <returns>The task object representing the asynchronous handling.</returns>
    Task HandleParsedProductPageAsync(
        ParsedProductPage parsedProductPage,
        Uri sourceUri,
        IProductPageProcessor productPageProcessor,
        CancellationToken ct = default);

    /// <summary>
    /// Updates the auction record with the <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Id of the ending auction record.</param>
    /// <param name="parsedProductPage">The current data of the auction.</param>
    /// <param name="sourceUri">The source URI of the parsed auction page.</param>
    /// <returns>The task object representing the asynchronous updating.</returns>
    Task UpdateAuctionRecordAsync(Guid id, ParsedProductPage parsedProductPage, Uri sourceUri);
}
