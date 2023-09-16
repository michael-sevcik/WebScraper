using WebScraper.Scraping;

namespace WebScraper.JobScheduling;

/// <summary>
/// Represents a class that schedules the update jobs.
/// </summary>
public interface IJobScheduler
{
    /// <summary>
    /// Schedules the job with the specified data.
    /// </summary>
    /// <param name="jobStart">When should the job start.</param>
    /// <param name="link">The source from which the job should be downloaded.</param>
    /// <param name="id">The id of the record.</param>
    /// <param name="pageProcessor">Processor that should parse the downloaded page.</param>
    /// <returns>A task object that represents the asynchronous operation.</returns>
    Task ScheduleUpdateJobAsync(DateTime jobStart, Uri link, Guid id, IProductPageProcessor pageProcessor);
}