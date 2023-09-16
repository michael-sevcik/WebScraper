using Downloader;
using HtmlAgilityPack;

namespace WebScraperTests.Mocks
{
    internal class MockHtmlDownloader : IHtmlDownloader
    {
        private readonly Dictionary<string, string> pagesByUri;

        public MockHtmlDownloader(Dictionary<string, string> pagesByUri)
            => this.pagesByUri = pagesByUri;

        public Task<HtmlDocument> GetPageDocumentAsync(Uri pageUri)
        {
            if (pagesByUri.TryGetValue(pageUri.AbsoluteUri, out var page))
            {
                HtmlDocument document = new();
                document.LoadHtml(page);
                return Task.FromResult(document);
            }
            else
            {
                throw new NetworkException("Page not found");
            }
        }

        public Task<Stream> GetStreamAsync(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
