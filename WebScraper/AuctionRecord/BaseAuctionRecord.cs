namespace WebScraper.AuctionRecord;

/// <summary>
/// Base implementation of any auction <c>record</c> entity.
/// </summary>
public class BaseAuctionRecord
{
    /// <summary>
    /// Gets the id of the record.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the record's time of creation.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// Gets the time when the auction was created.
    /// </summary>
    public DateTime StartOfAuction { get; init; }

    /// <summary>
    /// Gets the time when the auction will end.
    /// </summary>
    public DateTime EndOfAuction { get; init; }

    /// <summary>
    /// Gets or sets the date of the last modification of this record.
    /// </summary>
    public DateTime LastModification { get; set; }

    /// <summary>
    /// Gets or sets the price of the article that was known at the time specified in <see cref="LastModification"/>.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets the name of the auction.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier that is used for comparing ended auction records and newly scraped auctions.
    /// </summary>
    public string UniqueIdentification { get; init; } = string.Empty;

    /// <summary>
    /// Gets the additional information collection.
    /// </summary>
    public IReadOnlyCollection<KeyValuePair<string, string>> AdditionalInfromation { get; init; } = Array.Empty<KeyValuePair<string, string>>();
}