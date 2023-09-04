namespace WebScraper.Scraping;

/// <summary>
/// Base implementation of any auction <c>record</c> entity.
/// </summary>
public class ParsedProductPage
{
    /// <summary>
    /// Gets the record's time of creation.
    /// </summary>
    public DateTime Created { get; init; } = DateTime.Now;

    /// <summary>
    /// Gets the time when the auction will end.
    /// </summary>
    public DateTime EndOfAuction { get; init; }

    /// <summary>
    /// Gets or sets the price of the article that was known at the time specified in <see cref="LastModification"/>.
    /// </summary>
    public string Price { get; set; } = string.Empty;

    /// <summary>
    /// Gets the name of the auction.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier that is used for comparing ended auction records and newly scraped auctions.
    /// </summary>
    public string UniqueIdentifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets the additional information collection.
    /// </summary>
    public KeyValuePair<string, string>[] AdditionalInfromation { get; init; } = Array.Empty<KeyValuePair<string, string>>();
}