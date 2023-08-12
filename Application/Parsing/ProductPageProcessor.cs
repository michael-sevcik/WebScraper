using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using ProductListCrawler;
using WebScraper.AuctionRecord;
using WebScraper.Scraping;

namespace Application.Parsing;

/// <summary>
/// Implementation of <see cref="IProductPageProcessor"/> configurable via <see cref="ProductPageProcessorConfiguration"/>
/// </summary>
public class ProductPageProcessor : IProductPageProcessor
{
    private ProductPageProcessorConfiguration _configuration;

    /// <summary>
    /// Initializes an instance of <see cref="ProductPageProcessor"/> with the specified configuration.
    /// </summary>
    /// <param name="configuration">The product page processor configuration.</param>
    public ProductPageProcessor(ProductPageProcessorConfiguration configuration)
        => _configuration = configuration;

    /// <summary>
    /// Parses the product pages and extracts all the information into a <see cref="BaseAuctionRecord"/>.
    /// </summary>
    /// <param name="htmlDocument">The product page to be parsed.</param>
    /// <returns>The parsed information as an instance of <see cref="BaseAuctionRecord"/></returns>
    /// <exception cref="ParseException">An error occurred during parsing of <paramref name="htmlDocument"/>.</exception>
    public BaseAuctionRecord ParseProductPage(HtmlDocument htmlDocument)
    {
        try
        {
            var additionalInformation = this._configuration.AdditionalInfromation.Select(nameSelectorPair =>
            {
                var value = htmlDocument.QuerySelector(nameSelectorPair.CssSelector).InnerText; // TODO: CHECK
                return new KeyValuePair<string, string>(nameSelectorPair.Name, value);

            }).ToArray();

            return new BaseAuctionRecord() // TODO: finish the rest
            {
                Price = decimal.Parse(htmlDocument.QuerySelector(_configuration.PriceCssSelector).InnerText),
                Created = DateTime.Now,
                EndOfAuction = DateTime.ParseExact(htmlDocument.QuerySelector(
                    _configuration.EndOfAuctionCssSelector.Selector).InnerText,
                    _configuration.EndOfAuctionCssSelector.Format,
                    null),
                LastModification = DateTime.Now,
                Name = htmlDocument.QuerySelector(_configuration.NameCssSelector).InnerText,
                UniqueIdentification = htmlDocument.QuerySelector(_configuration.UniqueIdentificationCssSelector).InnerText,
                AdditionalInfromation = additionalInformation
            };
        }
        catch (Exception ex)
        {
            throw new ParseException(
                $"An error occurred during parsing of this product page:{Environment.NewLine}{htmlDocument.DocumentNode.InnerHtml}{Environment.NewLine}" +
                $"with this configuration:{Environment.NewLine}{_configuration}", ex);
        }
        
    }

    /// <inheritdoc/>
    public async Task<BaseAuctionRecord> ParseProductPageAsync(HtmlDocument htmlDocument)
        => await Task.Run(() => ParseProductPage(htmlDocument));
}
