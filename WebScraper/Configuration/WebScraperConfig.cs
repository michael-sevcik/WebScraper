using WebScraper.Scraping;

namespace WebScraper.Configuration;

/// <summary>
/// Encapsulates the configuration settings of a web scraper application.
/// </summary>
public sealed class WebScraperConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebScraperConfig"/> class
    /// with the <paramref name="scrapingJobs"/>.
    /// </summary>
    /// <param name="scrapingJobs">the list of product list URIs to be scraped.</param>
    /// <param name="sqlServerConnectionString">The db connection string.</param>
    public WebScraperConfig(IReadOnlyCollection<ScrapingJobDefinition> scrapingJobs, string sqlServerConnectionString)
        => (this.ScrapingJobs, this.SqlServerConnectionString) = (scrapingJobs, sqlServerConnectionString);

    /// <summary>
    /// Gets the period of scraping.
    /// </summary>
    public TimeSpan ScrapePeriod { get; init; } = TimeSpan.FromSeconds(300);

    /// <summary>
    /// Gets the list of sites to be scraped.
    /// </summary>
    public IReadOnlyCollection<ScrapingJobDefinition> ScrapingJobs { get; init; }

    /// <summary>
    /// Gets the period during which the auction product records are stored after the end of a given auction.
    /// </summary>
    public TimeSpan StoragePeriod { get; init; }

    /// <summary>
    /// Gets the db connection string.
    /// </summary>
    public string SqlServerConnectionString { get; init; }
}
