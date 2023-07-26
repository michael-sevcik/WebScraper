using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ProductListCrawler
{
    public interface IProductListCrawler
    {
        Task Crawl(Uri productListStart, ITargetBlock<IReadOnlyCollection<Uri>> productPageTarget);
    }
}
