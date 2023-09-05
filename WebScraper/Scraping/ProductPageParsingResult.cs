namespace WebScraper.Scraping;

/// <summary>
/// Encapsulates the parsed product page instance, its URI source and instance of the used <see cref="IProductPageProcessor"/>.
/// </summary>
public readonly struct ProductPageParsingResult
{
    /// <summary>
    /// The source of the <see cref="ParsedProductPage"/>.
    /// </summary>
    public readonly Uri Source;

    /// <summary>
    /// Output of the <see cref="ProductPageProcessor"/>.
    /// </summary>
    public readonly ParsedProductPage ParsedProductPage;

    /// <summary>
    /// Product page processor that was used to create <see cref="ParsedProductPage"/>.
    /// </summary>
    public readonly IProductPageProcessor ProductPageProcessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductPageParsingResult"/> struct.
    /// </summary>
    /// <param name="source">The source product page link.</param>
    /// <param name="parsedProductPage">The result of parsing page on <paramref name="source"/>.</param>
    /// <param name="productPageProcessor">The product page processor used to create <paramref name="parsedProductPage"/>.</param>
    public ProductPageParsingResult(Uri source, ParsedProductPage parsedProductPage, IProductPageProcessor productPageProcessor)
        => (this.Source, this.ParsedProductPage, this.ProductPageProcessor) = (source, parsedProductPage, productPageProcessor);
}
