using WebScraper.Configuration;
using WebScraper.Scraping;

namespace Application.Configuration;

/// <summary>
/// Encapsulates the configuration settings of a web scraper application.
/// </summary>
public sealed class WebScraperConfig : IReadOnlyWebScraperConfig // TODO: consider using record and making.
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebScraperConfig"/> class.
    /// </summary>
    public WebScraperConfig()
        => ScrapingJobs = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="WebScraperConfig"/> class
    /// with the <paramref name="scrapingJobs"/>.
    /// </summary>
    /// <param name="scrapingJobs">the list of product list URIs to be scraped.</param>
    public WebScraperConfig(List<ScrapingJobDefinition> scrapingJobs)
        => ScrapingJobs = scrapingJobs;

    /// <summary>
    /// Gets or sets the period of scraping.
    /// </summary>
    public TimeSpan ScrapePeriod { get; set; } = TimeSpan.FromSeconds(300);

    /// <summary>
    /// Gets or sets the list of sites to be scraped.
    /// </summary>
    public List<ScrapingJobDefinition> ScrapingJobs { get; set; }

    /// <summary>
    /// Gets or sets the period during which the auction product records are stored after the end of a given auction.
    /// </summary>
    public TimeSpan StoragePeriod { get; set; }

    /// <inheritdoc/>
    IReadOnlyList<ScrapingJobDefinition> IReadOnlyWebScraperConfig.ScrapingJobs => ScrapingJobs;
}
