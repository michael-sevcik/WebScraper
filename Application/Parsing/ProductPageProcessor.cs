using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using ProductListCrawler;
using WebScraper.Persistence.AuctionRecord;
using WebScraper.Scraping;

namespace Application.Parsing;

/// <summary>
/// Implementation of <see cref="IProductPageProcessor"/> configurable via <see cref="ProductPageProcessorConfiguration"/>
/// </summary>
public class ProductPageProcessor : IProductPageProcessor
{
    private readonly ProductPageProcessorConfiguration configuration;

    /// <summary>
    /// Initializes an instance of <see cref="ProductPageProcessor"/> with the specified configuration.
    /// </summary>
    /// <param name="configuration">The product page processor configuration.</param>
    public ProductPageProcessor(ProductPageProcessorConfiguration configuration)
        => this.configuration = configuration;

    /// <summary>
    /// Parses the product pages and extracts all the information into a <see cref="ParsedProductPage"/>.
    /// </summary>
    /// <param name="htmlDocument">The product page to be parsed.</param>
    /// <returns>The parsed information as an instance of <see cref="ParsedProductPage"/></returns>
    /// <exception cref="ParseException">An error occurred during parsing of <paramref name="htmlDocument"/>.</exception>
    public ParsedProductPage ParseProductPage(HtmlDocument htmlDocument)
    {
        try
        {
            var additionalInformation = this.configuration.AdditionalInfromation.Select(nameSelectorPair =>
            {
                var value = htmlDocument.QuerySelector(nameSelectorPair.CssSelector).InnerText;
                return new AdditionalInfromationPair() { Name = nameSelectorPair.Name, Value = value };

            }).ToList();

            var datestring = htmlDocument.QuerySelector(
                    configuration.EndOfAuctionCssSelector.Selector).InnerText;

            var endAuctionDate = DateTime.ParseExact(datestring,
                    configuration.EndOfAuctionCssSelector.Format,
                    null);

            return new ParsedProductPage()
            {
                Price = htmlDocument.QuerySelector(configuration.PriceCssSelector).InnerText,
                Created = DateTime.Now,
                EndOfAuction = endAuctionDate,
                Name = htmlDocument.QuerySelector(configuration.NameCssSelector).InnerText,
                UniqueIdentifier = htmlDocument.QuerySelector(configuration.UniqueIdentificationCssSelector).InnerText,
                AdditionalInfromation = additionalInformation
            };
        }
        catch (Exception ex)
        {
            throw new ParseException(
                $"An error occurred during parsing of this product page:{Environment.NewLine}{htmlDocument.DocumentNode.InnerHtml}{Environment.NewLine}" +
                $"with this configuration:{Environment.NewLine}{configuration}", ex);
        }
        
    }

    /// <inheritdoc/>
    public async Task<ParsedProductPage> ParseProductPageAsync(HtmlDocument htmlDocument)
        => await Task.Run(() => ParseProductPage(htmlDocument));
}
