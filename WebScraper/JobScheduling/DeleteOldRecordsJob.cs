using Quartz;
using WebScraper.Configuration;
using WebScraper.Persistence.UnitOfWork;
using WebScraper.Scraping;
using WebScraper.Utils;

namespace WebScraper.Jobs;

/// <summary>
/// Job that updates a record of an auction when the auction is about to end.
/// </summary>
/// <remarks>
/// The <see cref="IJobExecutionContext.MergedJobDataMap"/> is expected to contain record's <see cref="Guid"/> under "id" key,
/// <see cref="Uri"/> link to the product page under "link"
/// and appropriate <see cref="IProductPageLinkHandler"/> under "productPageLinkHandler".
/// </remarks>
internal sealed class DeleteOldRecordsJob : IJob
{
    /// <summary>
    /// The key of the quartz job.
    /// </summary>
    public static readonly JobKey Key = new(nameof(DeleteOldRecordsJob));

    /// <summary>
    /// The data map key to the storage period.
    /// </summary>
    public static readonly string StoragePeriodKey = "StoragePeriod";

    private readonly IUnitOfWorkProvider unitOfWorkProvider;
    private readonly IDateTimeProvider dateTimeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteOldRecordsJob"/> class.
    /// </summary>
    /// <param name="unitOfWorkProvider">The unit of work to be used in the execute method.</param>
    /// <param name="dateTimeProvider">The provider of date and time.</param>
    public DeleteOldRecordsJob(IUnitOfWorkProvider unitOfWorkProvider, IDateTimeProvider dateTimeProvider)
        => (this.unitOfWorkProvider, this.dateTimeProvider) = (unitOfWorkProvider, dateTimeProvider);

    /// <inheritdoc/>
    public async Task Execute(IJobExecutionContext context)
    {
        var storagePeriod = (TimeSpan)context.MergedJobDataMap[StoragePeriodKey];

        using var scopedUnitOfWork = this.unitOfWorkProvider.CreateScopedUnitOfWork();
        var repository = scopedUnitOfWork.UnitOfWork.AuctionRecordRepository;

        var recordsToDelete = await repository.GetAllEndingToDateAsync(this.dateTimeProvider.Now - storagePeriod);
        foreach (var record in recordsToDelete)
        {
            await repository.DeleteAsync(record.Id);
        }

        await scopedUnitOfWork.UnitOfWork.CompleteAsync();
    }
}
