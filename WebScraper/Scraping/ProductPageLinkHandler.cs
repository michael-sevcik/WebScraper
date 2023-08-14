using Downloader;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using ProductListCrawler;
using WebScraper.AuctionRecord;

namespace WebScraper.Scraping;

/// <inheritdoc/>
internal sealed class ProductPageLinkHandler : IProductPageLinkHandler
{
    private readonly ILogger<ProductPageLinkHandler> logger;
    private readonly IHtmlDownloader downloader;
    private readonly IProductPageProcessor productPageProcessor;
    private readonly IAuctionRecordManager recordManager;

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
        IProductPageProcessor productPageProcessor,
        IAuctionRecordManager recordManager)
        => (this.logger, this.downloader, this.productPageProcessor, this.recordManager) = (logger, downloader, productPageProcessor, recordManager);

    /// <inheritdoc/>
    public async Task HandleLinksAsync(IEnumerable<Uri> links)
    {
        foreach (Uri link in links)
        {
            var record = await this.GetAuctionRecord(link);
            if (record is not null)
            {
                await this.recordManager.HandleParsedProductPageAsync(record, link);
            }
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAuctionRecordAsync(Uri link, Guid id)
    {
        var record = await this.GetAuctionRecord(link, id);
        if (record is not null)
        {
            await this.recordManager.UpdateAuctionRecordAsync(id, record, link);
        }
    }

    private async Task<ParsedProductPage?> GetAuctionRecord(Uri link, Guid id = default)
    {
        // download the page
        HtmlDocument page;
        try
        {
            page = await this.downloader.GetPageDocumentAsync(link);
        }
        catch (NetworkException ex)
        {
            this.logger.LogError(ex.Message, ex.InnerException);
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
            this.logger.LogError(ex.Message, ex.InnerException);
            return null;
        }

        return parsingResult;
    }
}
