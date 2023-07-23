// Ignore Spelling: Downloader

namespace Downloader
{
    public static class Downloader
    {
        private static HttpClient client = new HttpClient();
        public static async Task<Stream> DownloadAsStringAsync(Uri uri)
        {
            return await client.GetStreamAsync(uri);
        }
    }
}