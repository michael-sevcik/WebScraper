using HtmlAgilityPack;

namespace Downloader;

/// <inheritdoc/>
public class HtmlDownloader : IHtmlDownloader
{
    private static readonly HttpClient client = new HttpClient();

    /// <inheritdoc/>
    public async Task<Stream> GetStreamAsync(Uri uri)
    {
        return await client.GetStreamAsync(uri);
    }

    /// <inheritdoc/>
    public async Task<HtmlDocument> GetPageDocumentAsync(Uri pageUri) // TODO: handle exceptions.
    {
        var productListPageStream = await this.GetStreamAsync(pageUri);
        return await Task.Run(() =>
        {
            HtmlDocument document = new();
            document.Load(productListPageStream);
            return document;
        });
    }
}