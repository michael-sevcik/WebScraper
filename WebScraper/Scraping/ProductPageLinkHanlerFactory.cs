using Downloader;
using Microsoft.Extensions.Logging;
using WebScraper.AuctionRecord;

namespace WebScraper.Scraping;

internal class ProductPageLinkHanlerFactory : IProductPageLinkHandlerFactory
{
    private readonly ILogger<ProductPageLinkHandler> logger;
    private readonly IHtmlDownloader downloader;
    private readonly IAuctionRecordManager recordManager;

    public ProductPageLinkHanlerFactory(
        ILogger<ProductPageLinkHandler> logger,
        IHtmlDownloader downloader,
        IAuctionRecordManager recordManager)
        => (this.logger, this.downloader, this.recordManager) = (logger, downloader, recordManager);

    public IProductPageLinkHandler Create(IProductPageProcessor productPageProcessor)
        => new ProductPageLinkHandler(this.logger, this.downloader, productPageProcessor, this.recordManager);
}
