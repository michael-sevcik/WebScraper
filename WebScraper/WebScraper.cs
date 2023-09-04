using System.Threading.Tasks.Dataflow;
using Downloader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductListCrawler;
using WebScraper.Configuration;
using WebScraper.Scraping;

namespace WebScraper;


/// <summary>
/// WebScraper class that handles scraping of web content.
/// </summary>
public class WebScraper
{
    private readonly ILogger<WebScraper> logger; // logger instance
    private readonly IProductListCrawler crawler; // product list crawler instance
    private readonly IProductPageLinkHandlerFactory productPageLinkHandlerFactory; // product page link handler factory instance
    private readonly WebScraperConfig config; // web scraper configuration instance

    /// <summary>
    /// Initializes a new instance of the <see cref="WebScraper"/> class with the specified dependencies..
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="crawler">The product list crawler instance.</param>
    /// <param name="factory">The product page link handler factory instance.</param>
    /// <param name="config">The web scraper configuration instance.</param>
    public WebScraper(
        ILogger<WebScraper> logger,
        IProductListCrawler crawler,
        IProductPageLinkHandlerFactory factory,
        WebScraperConfig config)
     => (this.logger, this.crawler, this.productPageLinkHandlerFactory, this.config)
        = (logger, crawler, factory, config);

    /// <summary>
    /// Executes the web scraping operations asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// Task object representing the asynchronous operation.
    /// </returns>
    public async Task ScrapeAsync(CancellationToken ct)
    {
        foreach (var job in this.config.ScrapingJobs)
        {
            var linkHandler = this.productPageLinkHandlerFactory.Create(job.ProductPageProcessor); // creating instance of product page link handler using factory instance
            ActionBlock<IReadOnlyCollection<Uri>> productPageLinkSink = new(
                async uris => { await linkHandler.HandleLinksAsync(uris, ct); },
                new() { CancellationToken = ct });

            // initialize buffer for holding product page links
            BufferBlock<IReadOnlyCollection<Uri>> buffer = new();
            using var bufferConsumerLink = buffer.LinkTo(productPageLinkSink); // links buffer to consumer

            foreach (var link in job.FirstProductListPageUris)
            {
                if (ct.IsCancellationRequested)
                {
                    return; // exit
                }

                // log scraping start
                this.logger.LogInformation($"Starting scraping of a product list page on URL \"{link.OriginalString}\"");

                // crawl product page and send content to buffer
                await this.crawler.Crawl(ct, link, job.ProductListProcessor, buffer);
            }

            // wait for completion of operations specified within time period
            await productPageLinkSink.Completion.WaitAsync(TimeSpan.FromMinutes(30));
        }
    }
}