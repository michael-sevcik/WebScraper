﻿using HtmlAgilityPack;

namespace Downloader;

/// <inheritdoc/>
public class HtmlDownloader : IHtmlDownloader
{
    private static readonly HttpClient client = new();

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
            throw new NetworkException($"An error occurred during creation of the stream. Requested URI: {uri}", ex);
        }

        return stream;
    }

    /// <inheritdoc/>
    public async Task<HtmlDocument> GetPageDocumentAsync(Uri pageUri)
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