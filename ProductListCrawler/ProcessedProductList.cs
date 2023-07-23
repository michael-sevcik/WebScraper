using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductListCrawler
{
    public struct ProcessedProductList
    {
        public IReadOnlyCollection<Uri> productPageUris;
        public Uri? nextPage;
    }
}
