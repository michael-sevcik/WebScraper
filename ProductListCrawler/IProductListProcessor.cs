namespace ProductListCrawler
{
    using HtmlAgilityPack;

    /// <summary>
    /// 
    /// </summary>
    public interface IProductListProcessor
    {
        // TODO: Define error reporting
        Task<ProcessedProductList> ProcessAsync(HtmlDocument productListPage);  
    }
}
