using System.Threading.Tasks.Dataflow;
using Downloader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using ProductListCrawler;
using WebScraper.Configuration;
using WebScraper.Persistence.UnitOfWork;
using WebScraper.Scraping;
using static Quartz.Logging.OperationName;

namespace WebScraper;

/// <summary>
/// WebScraper class that handles scraping of web content.
/// </summary>
public class WebScraper
{
    private readonly ILogger<WebScraper> logger; // logger instance
    private readonly IProductListCrawler crawler; // product list crawler instance
    private readonly IUnitOfWorkProvider unitOfWorkProvider;
    private readonly IProductPageLinkHandlerFactory productPageLinkHandlerFactory; // product page link handler factory instance
    private readonly WebScraperConfiguration config; // web scraper configuration instance

    /// <summary>
    /// Initializes a new instance of the <see cref="WebScraper"/> class with the specified dependencies..
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="crawler">The product list crawler instance.</param>
    /// <param name="unitOfWorkProvider">The unit of work provider.</param>
    /// <param name="factory">The product page link handler factory instance.</param>
    /// <param name="config">The web scraper configuration instance.</param>
    public WebScraper(
        ILogger<WebScraper> logger,
        IProductListCrawler crawler,
        IUnitOfWorkProvider unitOfWorkProvider,
        IProductPageLinkHandlerFactory factory,
        WebScraperConfiguration config)
     => (this.logger, this.crawler, this.unitOfWorkProvider, this.productPageLinkHandlerFactory, this.config)
        = (logger, crawler, unitOfWorkProvider, factory, config);

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
            var productPageLinkSink = this.CreateProductPageLinkSink(job, ct);

            // initialize buffer for holding product page links
            BufferBlock<IReadOnlyCollection<Uri>> buffer = new(new() { CancellationToken = ct });

            using var bufferConsumerLink = buffer.LinkTo(productPageLinkSink, new() { PropagateCompletion = true }); // links buffer to consumer

            foreach (var link in job.FirstProductListPageUris)
            {
                if (ct.IsCancellationRequested)
                {
                    return; // exit
                }

                // log scraping start
                this.logger.LogInformation("Starting scraping of a product list page on URL \"{OriginalString}\"", link.OriginalString);

                // crawl product page and send content to buffer
                try
                {
                    await this.crawler.Crawl(link, job.ProductListProcessor, buffer, ct);
                }
                catch (Exception ex)
                {
                    // log error
                    this.logger.LogError(
                        "Crawling of product list page with link {link} failed due to an error: {message}, {inner}",
                        link,
                        ex.Message,
                        ex.InnerException);
                }
            }

            buffer.Complete();

            await productPageLinkSink.Completion;
        }
    }

    private ActionBlock<IReadOnlyCollection<Uri>> CreateProductPageLinkSink(ScrapingJobDefinition jobDefinition, CancellationToken ct)
    {
        // Create a linkHandler using the predefined factory
        var linkHandler = this.productPageLinkHandlerFactory.Create(jobDefinition.ProductPageProcessor);

        // Create the sink
        ActionBlock<IReadOnlyCollection<Uri>> productPageLinkSink = new(
            async uris =>
            {
                // Get the scoped unit of work
                using var unitOfWorkScope = this.unitOfWorkProvider.CreateScopedUnitOfWork();
                var unitOfWork = unitOfWorkScope.UnitOfWork;

                var manager = unitOfWork.AuctionRecordManager;

                // Create an action block that will pass the parsed product pages to the manager
                ActionBlock<ProductPageParsingResult> productPageActionBlock = new(
                    async p =>
                    {
                        await manager.HandleParsedProductPageAsync(p.ParsedProductPage, p.Source, p.ProductPageProcessor, ct);
                    },
                    new()
                    {
                        CancellationToken = ct,
                        MaxDegreeOfParallelism = 1, // manager is not thread safe.
                    });

                BufferBlock<ProductPageParsingResult> bufferBlock = new();
                using var bufferActionBlockConnection = bufferBlock.LinkTo(productPageActionBlock, new() { PropagateCompletion = true });

                // Await the end of handling
                await linkHandler.HandleLinksAsync(uris, bufferBlock, ct);
                bufferBlock.Complete();

                await productPageActionBlock.Completion;

                if (ct.IsCancellationRequested)
                {
                    return;
                }

                await unitOfWork.CompleteAsync();
            },
            new() { CancellationToken = ct });

        return productPageLinkSink;
    }
}