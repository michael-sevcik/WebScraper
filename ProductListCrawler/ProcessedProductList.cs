using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductListCrawler
{
    /// <summary>
    /// The output of product list processing defined by <see cref="IProductListProcessor"/>.
    /// </summary>
    public struct ProcessedProductList
    {
        /// <summary>
        /// URIs found on the product list page as references to product pages.
        /// </summary>
        public IReadOnlyCollection<Uri> productPageUris;

        /// <summary>
        /// Address of next product list page.
        /// </summary>
        /// <value><see langword="null"/> if next product list page does not exist.</value>
        public string? nextPage;
    }
}
