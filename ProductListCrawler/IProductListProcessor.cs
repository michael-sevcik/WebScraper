namespace ProductListCrawler
{
    using HtmlAgilityPack;
    public interface IProductListProcessor
    {
        Task<ProcessedProductList> ProcessAsync(HtmlDocument productListPage);  
    }
}
