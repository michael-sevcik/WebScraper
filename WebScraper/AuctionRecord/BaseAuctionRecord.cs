namespace WebScraper.AuctionRecord;

/// <summary>
/// Encapsulates field value pair used for storing information.
/// </summary>
/// <param name="Name">The name of the field.</param>
/// <param name="Value">The value of the field.</param>
public record struct AdditionalFieldValuePair(string Name, string Value); // TODO: MOVE

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
    /// Gets or sets the additional information collection.
    /// </summary>
    public IReadOnlyCollection<AdditionalFieldValuePair> AdditionalInfromation { get; set; }
}
