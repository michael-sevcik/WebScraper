// Ignore Spelling: downloader

namespace ProductListCrawler;

using Downloader;
using HtmlAgilityPack;
using System.Threading.Tasks.Dataflow;
using System.Linq;
using System.Collections.Concurrent;


public class ProductListCrawler : IProductListCrawler
{
    private readonly IProductListProcessor processor;
    private readonly IHtmlDownloader downloader;

    public ProductListCrawler(IHtmlDownloader downloader, IProductListProcessor processor)
    {
        this.downloader = downloader;
        this.processor = processor;
    }

    public async Task Crawl(Uri productListStart, ITargetBlock<IReadOnlyCollection<Uri>> productPageTarget)
    {
        // TODO: Handle Exceptions
        Uri? nextProductPage = productListStart;
        do
        {
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