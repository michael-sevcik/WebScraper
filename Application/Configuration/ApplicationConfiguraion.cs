﻿using Application.Parsing;
using WebScraper.Configuration;
using WebScraper.Scraping;

namespace Application.Configuration
{
    /// <summary>
    /// A serializable class serving as encapsulation of the all configurations that are meant to be stored.
    /// </summary>
    [Serializable]
    public sealed class ApplicationConfiguration // TODO: build the serializable configuration - the job replace the job definitions with processor configurations
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebScraperConfig"/> class.
        /// </summary>
        public ApplicationConfiguration()
            => this.ScrapingJobs = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebScraperConfig"/> class
        /// with the <paramref name="scrapingJobs"/>.
        /// </summary>
        /// <param name="scrapingJobs">the list of product list URIs to be scraped.</param>
        public ApplicationConfiguration(List<SerializableScrapingJobDefinition> scrapingJobs)
            => this.ScrapingJobs = scrapingJobs;

        /// <summary>
        /// Gets or sets the period of scraping.
        /// </summary>
        public TimeSpan ScrapePeriod { get; set; } = TimeSpan.FromSeconds(300);

        /// <summary>
        /// Gets or sets the list of sites to be scraped.
        /// </summary>
        public List<SerializableScrapingJobDefinition> ScrapingJobs { get; set; }

        /// <summary>
        /// Gets or sets the period during which the auction product records are stored after the end of a given auction.
        /// </summary>
        public TimeSpan StoragePeriod { get; set; }

        /// <summary>
        /// Builds <see cref="WebScraperConfig"/> from the data in this instance of <see cref="ApplicationConfiguration"/>
        /// </summary>
        /// <returns></returns>
        internal WebScraperConfig BuildWebScraperConfiguration()
        {

            var scrapingJobs = this.ScrapingJobs.Select(jobDefinition => new ScrapingJobDefinition(
                jobDefinition.FirstProductListPageUris,
                new ProductListProcessor(jobDefinition.ListProcessorConfiguration),
                new ProductPageProcessor(jobDefinition.PageProcessorConfiguration))).ToArray();

            return new(scrapingJobs)
            {
                ScrapePeriod = this.ScrapePeriod,
                StoragePeriod = this.StoragePeriod,
            };
        }
    }
}
