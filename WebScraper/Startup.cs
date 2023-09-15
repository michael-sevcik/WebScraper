using Downloader;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductListCrawler;
using Quartz;
using WebScraper.Configuration;
using WebScraper.Jobs;
using WebScraper.JobScheduling;
using WebScraper.Notifications;
using WebScraper.Persistence;
using WebScraper.Persistence.AuctionRecord;
using WebScraper.Persistence.UnitOfWork;
using WebScraper.Scraping;
using WebScraper.Utils;

namespace WebScraper;

/// <summary>
/// <see cref="WebScraper"/> startup class.
/// </summary>
public class Startup
{
    private readonly WebScraperConfiguration config;

    private readonly INotifier? specifiedNotifier;

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="config">The configuration to add.</param>
    /// <param name="notifier">The notifier that handles sending of item readdition notifications.</param>
    public Startup(WebScraperConfiguration config, INotifier? notifier = null)
        => (this.config, this.specifiedNotifier) = (config, notifier);

    /// <summary>
    /// Configures the host builder to do web scraping according to the provided configuration.
    /// </summary>
    /// <param name="hostBuilder">The host builder to configure.</param>
    public void ConfigureHostBuilder(IHostBuilder hostBuilder)
        => hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton(this.config);
            services.AddSingleton<IHtmlDownloader, HtmlDownloader>();
            services.AddSingleton<IProductListCrawler, ProductListCrawler.ProductListCrawler>();

            // Use the specified notifier if it was provided, otherwise use the default logging one.
            if (this.specifiedNotifier is not null)
            {
                services.AddSingleton(this.specifiedNotifier);
            }
            else
            {
                services.AddSingleton<INotifier, LogNotifier>();
            }

            services.AddDbContext<ScraperDbContext>(
            options => options.UseSqlServer(this.config.SqlServerConnectionString));

            services.AddScoped<IAuctionRecordRepository, DbAuctionRecordRepository>();
            services.AddScoped<IAuctionRecordManager, AuctionRecordManager>();
            services.AddScoped<IUnitOfWork, ScraperUnitOfWork>();

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IUnitOfWorkProvider, UnitOfWorkProvider>();
            services.AddSingleton<JobScheduler>();
            services.AddSingleton<IProductPageLinkHandlerFactory, ProductPageLinkHandlerFactory>();
            services.AddSingleton<WebScraper>();

            this.AddQuartzServices(services);
        });

    private void AddQuartzServices(IServiceCollection services)
    {
        // Add the required Quartz.NET services
        services.AddQuartz(q =>
        {
            // Register the jobs
            q.AddJob<AuctionEndingUpdateJob>(AuctionEndingUpdateJob.Key, c => { c.StoreDurably(true); }); // It has no trigger directly attached.
            q.AddJob<CheckAuctionListsJob>(CheckAuctionListsJob.Key);

            JobDataMap deleteJobDataMap = new() { { DeleteOldRecordsJob.StoragePeriodKey, this.config.StoragePeriod } };
            q.AddJob<DeleteOldRecordsJob>(DeleteOldRecordsJob.Key, c => c.UsingJobData(deleteJobDataMap));

            // Create a trigger for the main scraping job
            q.AddTrigger(opts => opts
                .ForJob(DeleteOldRecordsJob.Key) // link to the delete job
                .WithIdentity($"{DeleteOldRecordsJob.Key}-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromDays(1))
                    .RepeatForever()));

            q.AddTrigger(opts => opts
                .ForJob(CheckAuctionListsJob.Key) // link to the main scraping job
                .WithIdentity($"{CheckAuctionListsJob.Key}-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(this.config.ScrapePeriod)
                    .RepeatForever()));
        });

        // Add the Quartz.NET hosted service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
