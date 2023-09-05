using System.Threading.Tasks.Dataflow;
using Downloader;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using ProductListCrawler;

namespace WebScraper.Scraping;

/// <inheritdoc/>
internal sealed class ProductPageLinkHandler : IProductPageLinkHandler
{
    private readonly ILogger<ProductPageLinkHandler> logger;
    private readonly IHtmlDownloader downloader;
    private readonly IProductPageProcessor productPageProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductPageLinkHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="downloader">The downloader for getting new data.</param>
    /// <param name="productPageProcessor">The parser that should be used for parsing the downloaded data.</param>
    /// <param name="recordManager">The manager which handles the new records. </param>
    public ProductPageLinkHandler(
        ILogger<ProductPageLinkHandler> logger,
        IHtmlDownloader downloader,
        IProductPageProcessor productPageProcessor)
        => (this.logger, this.downloader, this.productPageProcessor) = (logger, downloader, productPageProcessor);

    /// <inheritdoc/>
    public async Task HandleLinksAsync(
        IEnumerable<Uri> links,
        ITargetBlock<ProductPageParsingResult> targetBlock,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Handling link block", links);

        await Parallel.ForEachAsync(links, cancellationToken, async (link, ct) =>
        {
            if (ct.IsCancellationRequested)
            {
                return;
            }

            var record = await this.GetAuctionRecord(link);
            if (record is null || ct.IsCancellationRequested)
            {
                return;
            }

            if (!await targetBlock.SendAsync(new(link, record, this.productPageProcessor), ct))
            {
                throw new Exception("Consumer is missing");
            }
        });

        this.logger.LogTrace("Finished handling link block");
    }

    /// <inheritdoc/>
    public async Task<ParsedProductPage?> GetAuctionRecord(Uri link)
    {
        // download the page
        HtmlDocument page;
        try
        {
            page = await this.downloader.GetPageDocumentAsync(link);
        }
        catch (NetworkException ex)
        {
            this.logger.LogError("Exception occured: {message}, inner: {inner}", ex.Message, ex.InnerException);
            return null;
        }

        // parse the page
        ParsedProductPage parsingResult;
        try
        {
            parsingResult = await this.productPageProcessor.ParseProductPageAsync(page);
        }
        catch (ParseException ex)
        {
            this.logger.LogError("Exception occured: {message}, inner: {inner}", ex.Message, ex.InnerException);
            return null;
        }

        return parsingResult;
    }
}
