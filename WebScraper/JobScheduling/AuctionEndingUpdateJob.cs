using Quartz;
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
    /// <inheritdoc/>
    public async Task Execute(IJobExecutionContext context)
    {
        var link = (Uri)context.MergedJobDataMap["link"];
        var id = context.MergedJobDataMap.GetGuid("id");
        var productPagelinkHandler = (IProductPageLinkHandler)context.MergedJobDataMap["productPageLinkHandler"];

        await productPagelinkHandler.UpdateAuctionRecordAsync(link, id);
    }
}
