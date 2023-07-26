// Ignore Spelling: Downloader

namespace Downloader;

public class Downloader : IDownloader
{
    private static readonly HttpClient client = new HttpClient();
    public async Task<Stream> GetStreamAsync(Uri uri)
    {
        return await client.GetStreamAsync(uri);
    }
}