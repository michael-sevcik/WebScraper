namespace ProductListCrawler
{
    using Downloader;
    using HtmlAgilityPack;
    using System.Threading.Tasks.Dataflow;
    using System.Linq;

    public class ProductListCrawler
    {
        private readonly IProductListProcessor processor;

        public ProductListCrawler(IProductListProcessor processor)
        {
            this.processor = processor;
        }

        public async Task Crawl(Uri ProductListStart, ITargetBlock<Uri> productPageTarget)
        {
            // TODO: Handle Exceptions
            Uri? nextProductPage = ProductListStart;
            do
            {
                var output = await processor.ProcessAsync(await GetPageDocument(nextProductPage));
                foreach (var productPageUri in output.productPageUris)
                {
                    // TODO:
                }

                nextProductPage = output.nextPage;
            } while (nextProductPage != null);
        }

    private static async Task<HtmlDocument> GetPageDocument(Uri ProductListStart)
    {
        var productListPageStream = await Downloader.GetStreamAsync(ProductListStart);
        return await Task.Run(() =>
        {
            HtmlDocument document = new();
            document.Load(productListPageStream);
            return document;
        });

    }

        }

    }
}