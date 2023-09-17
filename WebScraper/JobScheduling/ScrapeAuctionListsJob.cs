using Quartz;

namespace WebScraper.Jobs;

/// <summary>
/// Job that scrapes the auction lists, stores the new auctions and notifies about possible readded auction items.
/// </summary>
internal class ScrapeAuctionListsJob : IJob
{
    /// <summary>
    /// The key of the quartz job.
    /// </summary>
    public static readonly JobKey Key = new(nameof(ScrapeAuctionListsJob));
    private readonly WebScraper webScraper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrapeAuctionListsJob"/> class.
    /// </summary>
    /// <param name="webScraper">The web scraper to run.</param>
    public ScrapeAuctionListsJob(WebScraper webScraper)
        => this.webScraper = webScraper;

    /// <inheritdoc/>
    public async Task Execute(IJobExecutionContext context)
    {
        await this.webScraper.ScrapeAsync(context.CancellationToken);
    }
}
