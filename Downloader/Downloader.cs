// Ignore Spelling: Downloader

namespace Downloader;

/// <inheritdoc/>
public class Downloader : IDownloader
{
    private static readonly HttpClient client = new HttpClient();

    /// <inheritdoc/>
    public async Task<Stream> GetStreamAsync(Uri uri)
    {
        return await client.GetStreamAsync(uri);
    }
}