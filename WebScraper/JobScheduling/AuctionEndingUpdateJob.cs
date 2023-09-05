using Quartz;
using WebScraper.Persistence.UnitOfWork;
using WebScraper.Scraping;

namespace WebScraper.Jobs;

/// <summary>
/// Job that updates a record of an auction when the auction is about to end.
/// </summary>
/// <remarks>
/// The <see cref="IJobExecutionContext.MergedJobDataMap"/> is expected to contain record's <see cref="Guid"/> under "id" key,
/// <see cref="Uri"/> link to the product page under "link"
/// and appropriate <see cref="IProductPageLinkHandler"/> under "productPageLinkHandler".
/// </remarks>
internal sealed class AuctionEndingUpdateJob : IJob
{
    /// <summary>
    /// The key of the quartz job.
    /// </summary>
    public static readonly JobKey Key = new(nameof(AuctionEndingUpdateJob));

    /// <summary>
    /// The data map id key.
    /// </summary>
    public static readonly string RecordIdKey = "RecordId";

    /// <summary>
    /// The data map key of a saved  <see cref="IProductPageProcessor"/> instance.
    /// </summary>
    public static readonly string PageProcessorKey = "PageProcessor";

    /// <summary>
    /// The data map link key.
    /// </summary>
    public static readonly string RecordSourceLinkKey = "RecordSourceLink";

    private readonly IUnitOfWork unitOfWork;
    private readonly IProductPageLinkHandlerFactory linkHandlerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionEndingUpdateJob"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to be used in the execute method.</param>
    /// <param name="linkHandlerFactory">Factory to create the link handler.</param>
    public AuctionEndingUpdateJob(IUnitOfWork unitOfWork, IProductPageLinkHandlerFactory linkHandlerFactory)
        => (this.unitOfWork, this.linkHandlerFactory) = (unitOfWork, linkHandlerFactory);

    /// <inheritdoc/>
    public async Task Execute(IJobExecutionContext context)
    {
        // data from the data map
        var link = (Uri)context.MergedJobDataMap[RecordSourceLinkKey];
        var id = context.MergedJobDataMap.GetGuid(RecordIdKey);
        var pageProcessor = (IProductPageProcessor)context.MergedJobDataMap[PageProcessorKey];

        // Create link handler using predefined link handler factory
        var linkHandler = this.linkHandlerFactory.Create(pageProcessor);

        // Get the product page and pass it to the manager.
        var parsedProductPage = await linkHandler.GetAuctionRecord(link);

        // retry getting the page two more times
        int i = 0;
        while (parsedProductPage is null && i++ < 2)
        {
            parsedProductPage = await linkHandler.GetAuctionRecord(link);
            await Task.Delay(500);
        }

        // if product page was successfully parsed, pass it to the auction record manager
        using (this.unitOfWork)
        {
            if (parsedProductPage is null)
            {
                return;
            }

            await this.unitOfWork.AuctionRecordManager.UpdateAuctionRecordAsync(id, parsedProductPage, link);
            await this.unitOfWork.CompleteAsync();
        }
    }
}
