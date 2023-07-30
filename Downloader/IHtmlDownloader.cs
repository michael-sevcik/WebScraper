using HtmlAgilityPack;

namespace Downloader;

/// <summary>
/// Represents a helper class that provides easy way to download content from the Internet.
/// </summary>
public interface IHtmlDownloader
{
    /// <summary>
    /// Sends a GET request to the <paramref name="uri"/> and provides the response body stream
    /// as an asynchronous operation.
    /// </summary>
    /// <param name="uri">The URI to which is the request sent.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The Result property on the task object returns the Get response body as stream.
    /// </returns>
    Task<Stream> GetStreamAsync(Uri uri);

    /// <summary>
    /// Sends a GET request to the <paramref name="uri"/> and provides the response body parsed 
    /// in <see cref="HtmlDocument"/> as an asynchronous operation.
    /// </summary>
    /// <param name="pageUri">The URI to which is the request sent.</param>
    /// <returns>
    /// The task object representing the asynchronous operation.
    /// The Result property on the task object returns the Get response body as <see cref="HtmlDocument"/>.
    /// </returns>
    Task<HtmlDocument> GetPageDocumentAsync(Uri pageUri);
}
