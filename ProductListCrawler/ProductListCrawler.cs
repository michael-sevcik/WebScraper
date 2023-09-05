// Ignore Spelling: downloader

namespace ProductListCrawler;

using Downloader;
using HtmlAgilityPack;
using System.Threading.Tasks.Dataflow;
using System.Linq;
using System.Collections.Concurrent;

/// <inheritdoc/>
public class ProductListCrawler : IProductListCrawler
{
    private readonly IHtmlDownloader downloader;

    /// <summary>
    /// Creates a new instance of <see cref="ProductListCrawler"/>.
    /// </summary>
    /// <param name="downloader">THe downloader to use for downloading product list pages.</param>
    public ProductListCrawler(IHtmlDownloader downloader)
        => this.downloader = downloader;

    /// <inheritdoc/>
    public async Task Crawl(Uri productListStart, IProductListProcessor processor, ITargetBlock<IReadOnlyCollection<Uri>> productPageTarget, CancellationToken token)
    {
        Uri? nextProductPage = productListStart;
        do
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var output = await processor.ProcessAsync(await downloader.GetPageDocumentAsync(nextProductPage));
            if (output.productPageUris.Any())
            {
                if (!await productPageTarget.SendAsync(output.productPageUris))
                {
                    throw new Exception("Consumer missing.");
                }
            }

            nextProductPage = CombineUris(productListStart, output.nextPage);
        } while (nextProductPage != null);
    }

    private static Uri? CombineUris(Uri baseUri, string? rest)
    {
        if (Uri.IsWellFormedUriString(rest, UriKind.Absolute))
        {
            return new Uri(rest);
        }

        Uri.TryCreate(baseUri, rest, out var result);
        return result;
    }
}