using Downloader;
using Microsoft.Extensions.Logging;

namespace WebScraper.Scraping;

/// <summary>
/// Implementation of the <see cref="IProductPageLinkHandlerFactory"/>.
/// </summary>
internal class ProductPageLinkHandlerFactory : IProductPageLinkHandlerFactory
{
    private readonly ILogger<ProductPageLinkHandler> logger;
    private readonly IHtmlDownloader downloader;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductPageLinkHandlerFactory"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="downloader">The downloader for the <see cref="ProductPageLinkHandler"/>s.</param>
    public ProductPageLinkHandlerFactory(
        ILogger<ProductPageLinkHandler> logger,
        IHtmlDownloader downloader)
        => (this.logger, this.downloader) = (logger, downloader);

    /// <inheritdoc/>
    public IProductPageLinkHandler Create(IProductPageProcessor productPageProcessor)
    {
        return new ProductPageLinkHandler(this.logger, this.downloader, productPageProcessor);
    }
}
