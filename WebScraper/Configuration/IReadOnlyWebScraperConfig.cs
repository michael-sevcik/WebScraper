using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebScraper.Scraping;

namespace WebScraper.Configuration;

/// <summary>
/// Represents a view on the configuration settings of a web scraper application.
/// </summary>
public interface IReadOnlyWebScraperConfig
{
    /// <summary>
    /// Gets the period of scraping.
    /// </summary>
    public TimeSpan ScrapePeriod { get; }

    /// <summary>
    /// Gets the list of sites to be scraped.
    /// </summary>
    public IReadOnlyList<ScrapingJobDefinition> ScrapingJobs { get; }
}
