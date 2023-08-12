using HtmlAgilityPack;

namespace Downloader;

/// <inheritdoc/>
public class HtmlDownloader : IHtmlDownloader
{
    private static readonly HttpClient client = new HttpClient();

    /// <inheritdoc/>
    public async Task<Stream> GetStreamAsync(Uri uri)
    {
        Stream stream;
        try
        {
            stream =  await client.GetStreamAsync(uri);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new NetworkException("An error occured during creation of the stream", ex);
        }

        return stream;
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