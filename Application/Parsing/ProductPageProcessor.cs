using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using WebScraper.AuctionRecord;
using WebScraper.Scraping;

namespace Application.Parsing
{
    public class ProductPageProcessor : IProductPageProcessor
    {
        private readonly XPathExpression _endOfAuction;
        private readonly XPathExpression _uniqueIdentification;
        private readonly XPathExpression _price;
        private readonly XPathExpression _name;
        private readonly IEnumerable<KeyValuePair<string, XPathExpression>> _additionalInfromation;

        public ProductPageProcessor(ProductPageProcessorConfiguration configuration)
        {
            _endOfAuction = XPathExpression.Compile(configuration.EndOfAuctionXPathSelector);
            _uniqueIdentification = XPathExpression.Compile(configuration.UniqueIdentificationXPathSelector);
            _price = XPathExpression.Compile(configuration.PriceXPathSelector);
            _name = XPathExpression.Compile(configuration.NameXPathSelector);

            _additionalInfromation = configuration.AdditionalInfromation.Select(nameSelectorPair =>
                new KeyValuePair<string, XPathExpression>(
                    nameSelectorPair.Name,
                    XPathExpression.Compile(nameSelectorPair.XPathSelector)));
        }

        /// <summary>
        /// Parses the product pages and extracts all the information into a <see cref="BaseAuctionRecord"/>.
        /// </summary>
        /// <param name="htmlDocument">The product page to be parsed.</param>
        /// <returns>The parsed information as an instance of <see cref="BaseAuctionRecord"/></returns>
        public BaseAuctionRecord ParseProductPage(HtmlDocument htmlDocument)
        {
            throw new NotImplementedException(); // TODO:
        }

        /// <inheritdoc/>
        public async Task<BaseAuctionRecord> ParseProductPageAsync(HtmlDocument htmlDocument)
            => await Task.Run(() => ParseProductPage(htmlDocument));
    }
}
