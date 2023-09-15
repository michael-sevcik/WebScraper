using Microsoft.Extensions.Logging;
using Quartz;
using WebScraper.Jobs;
using WebScraper.Scraping;

namespace WebScraper.JobScheduling;

/// <summary>
/// Scheduler for scheduling update jobs. <see cref="AuctionEndingUpdateJob"/>.
/// </summary>
internal sealed class JobScheduler
{
    private static readonly TimeSpan TimeReserve = TimeSpan.FromSeconds(70);
    private readonly ILogger<JobScheduler> logger;
    private readonly IScheduler scheduler;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobScheduler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="schedulerFactory">
    /// A factory that should provide schedulers for scheduling jobs.
    /// </param>
    public JobScheduler(
        ILogger<JobScheduler> logger,
        ISchedulerFactory schedulerFactory)
    {
        this.logger = logger;

        var schedulerTask = schedulerFactory.GetScheduler();
        schedulerTask.Wait();
        this.scheduler = schedulerTask.Result;
    }

    /// <summary>
    /// Schedules the job with the specified data.
    /// </summary>
    /// <param name="jobStart">When should the job start.</param>
    /// <param name="link">The source from which the job should be downloaded.</param>
    /// <param name="id">The id of the record.</param>
    /// <param name="pageProcessor">Processor that should parse the downloaded page.</param>
    /// <returns>A task object that represents the asynchronous operation.</returns>
    public async Task ScheduleUpdateJobAsync(
        DateTime jobStart,
        Uri link,
        Guid id,
        IProductPageProcessor pageProcessor)
    {
        // Populate the data map
        JobDataMap jobDataMap = new()
        {
            { AuctionEndingUpdateJob.RecordSourceLinkKey, link },
            { AuctionEndingUpdateJob.RecordIdKey, id },
            { AuctionEndingUpdateJob.PageProcessorKey, pageProcessor },
        };

        var startTime = jobStart - TimeReserve;

        // Create a trigger
        var trigger = TriggerBuilder.Create()
            .StartAt(startTime)
            .UsingJobData(jobDataMap)
            .ForJob(AuctionEndingUpdateJob.Key)
            .Build();

        this.logger.LogInformation($"Scheduling an update job for the product on page {link}. The job start: {startTime}");

        // Schedule the job
        await this.scheduler.ScheduleJob(trigger);
    }
}
