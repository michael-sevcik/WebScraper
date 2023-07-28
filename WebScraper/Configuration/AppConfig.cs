namespace WebScraper.Configuration;

using Scraping;

/// <summary>
/// Encapsulates the configuration settings of a web scraper application.
/// </summary>
internal class AppConfig : IReadOnlyAppConfig // TODO: consider using record.
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfig"/> class.
    /// </summary>
    public AppConfig()
        => this.ScrapingJobs = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AppConfig"/> class
    /// with the <paramref name="scrapingJobs"/>.
    /// </summary>
    /// <param name="scrapingJobs">the list of product list URIs to be scraped.</param>
    public AppConfig(List<ScrapingJobDefinition> scrapingJobs)
        => this.ScrapingJobs = scrapingJobs;

    /// <summary>
    /// Gets or sets the period of scraping. // TODO: Consider using timespan.
    /// </summary>
    public TimeSpan ScrapePeriod { get; set; }

    /// <summary>
    /// Gets or sets the list of sites to be scraped.
    /// </summary>
    public List<ScrapingJobDefinition> ScrapingJobs { get; set; }

    /// <inheritdoc/>
    IReadOnlyList<ScrapingJobDefinition> IReadOnlyAppConfig.ScrapingJobs => this.ScrapingJobs;
}
