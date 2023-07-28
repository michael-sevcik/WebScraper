using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Configuration;

using Scraping;

/// <summary>
/// Represents a view on the configuration settings of a web scraper application.
/// </summary>
public interface IReadOnlyAppConfig
{
    /// <summary>
    /// Gets the period of scraping. // TODO: Consider using timespan.
    /// </summary>
    public TimeSpan ScrapePeriod { get; }

    /// <summary>
    /// Gets the list of sites to be scraped.
    /// </summary>
    public IReadOnlyList<ScrapingJobDefinition> ScrapingJobs { get; }
}
