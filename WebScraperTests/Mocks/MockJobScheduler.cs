using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.JobScheduling;
using WebScraper.Scraping;

namespace WebScraperTests.Mocks;

internal class MockJobScheduler : IJobScheduler
{
    private readonly List<JobSchedulingDetails> scheduledJobs = new();

    public record JobSchedulingDetails(DateTime jobStart, Uri link);

    public JobSchedulingDetails[] ScheduledJobs => this.scheduledJobs.OrderBy(x => x.link.AbsoluteUri).ToArray();

    public Task ScheduleUpdateJobAsync(DateTime jobStart, Uri link, Guid id, IProductPageProcessor pageProcessor)
    {
        this.scheduledJobs.Add(new(jobStart, link));
        return Task.CompletedTask;
    }
}
