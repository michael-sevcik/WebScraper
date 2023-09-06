using Application.Parsing;
using MailSender;
using WebScraper.Configuration;
using WebScraper.Scraping;

namespace Application.Configuration
{
    /// <summary>
    /// A serializable class serving as encapsulation of the all configurations that are meant to be stored.
    /// </summary>
    [Serializable]
    public sealed class SerializableWebScraperConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebScraperConfiguration"/> class.
        /// </summary>
        public SerializableWebScraperConfiguration()
            => (this.ScrapingJobs) = (new());

        /// <summary>
        /// Initializes a new instance of the <see cref="WebScraperConfiguration"/> class
        /// with the <paramref name="scrapingJobs"/>.
        /// </summary>
        /// <param name="scrapingJobs">the list of product list URIs to be scraped.</param>
        public SerializableWebScraperConfiguration(List<SerializableScrapingJobDefinition> scrapingJobs)
            => (this.ScrapingJobs) = (scrapingJobs);

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
        /// Gets or sets the SQL server connection string.
        /// </summary>
        public string DbConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Builds <see cref="WebScraperConfiguration"/> from the data in this instance of <see cref="SerializableWebScraperConfiguration"/>
        /// </summary>
        /// <returns></returns>
        internal WebScraperConfiguration BuildWebScraperConfiguration()
        {

            var scrapingJobs = this.ScrapingJobs.Select(jobDefinition => new ScrapingJobDefinition(
                jobDefinition.FirstProductListPageUris,
                new ProductListProcessor(jobDefinition.ListProcessorConfiguration),
                new ProductPageProcessor(jobDefinition.PageProcessorConfiguration))).ToArray();

            return new(scrapingJobs, this.DbConnectionString)
            {
                ScrapePeriod = this.ScrapePeriod,
                StoragePeriod = this.StoragePeriod,
            };
        }
    }
}
