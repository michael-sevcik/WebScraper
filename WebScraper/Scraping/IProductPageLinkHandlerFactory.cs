using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Scraping;

/// <summary>
/// Represents a productPageLinkHandlerFactory class for creating instances of <see cref="IProductPageLinkHandler"/>.
/// </summary>
internal interface IProductPageLinkHandlerFactory
{
    /// <summary>
    /// Creates a link handler that uses a specified <paramref name="productPageProcessor"/>.
    /// </summary>
    /// <param name="productPageProcessor">The product page processor that the handler should use.</param>
    /// <returns>The configured link handler.</returns>
    IProductPageLinkHandler Create(IProductPageProcessor productPageProcessor);
}
