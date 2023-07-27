namespace ProductListCrawler
{
    using HtmlAgilityPack;

    /// <summary>
    /// Represents a class that serves to process the product list pages.
    /// </summary>
    public interface IProductListProcessor
    {
        /// <summary>
        /// Processes the <paramref name="productListPage"/> as an asynchronous operation.
        /// </summary>
        /// <param name="productListPage">The <c>HTML page</c> to be processed.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task 
        /// object returns <see cref="ProcessedProductList"/> that encapsulates the processed data.
        /// </returns>
        Task<ProcessedProductList> ProcessAsync(HtmlDocument productListPage);  
    }
}
