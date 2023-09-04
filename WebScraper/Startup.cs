using Downloader;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductListCrawler;
using Quartz;
using WebScraper.Configuration;
using WebScraper.Jobs;
using WebScraper.Notifications;
using WebScraper.Persistence;
using WebScraper.Persistence.AuctionRecord;
using WebScraper.Scraping;

namespace WebScraper;

/// <summary>
/// <see cref="WebScraper"/> startup class.
/// </summary>
public class Startup
{
    private readonly WebScraperConfig config;

    private readonly INotifier? specifiedNotifier;

    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="config">The configuration to add.</param>
    /// <param name="notifier">The notifier that handles sending of item readdition notifications.</param>
    public Startup(WebScraperConfig config, INotifier? notifier = null)
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

            services.AddDbContext<ScraperDbContext>(
            options => options.UseSqlServer(this.config.SqlServerConnectionString), ServiceLifetime.Transient);
            services.AddTransient<IAuctionRecordRepository, DbAuctionRecordRepository>();

            //services.AddTransient<IAuctionRecordRepository, RamAuctionRecordRepository>(); // todo: delete

            // Use the specified notifier if it was provided, otherwise use the default logging one.
            if (this.specifiedNotifier is not null)
            {
                services.AddSingleton(this.specifiedNotifier); // TODO: Check thread safety
            }
            else
            {
                services.AddSingleton<INotifier, LogNotifier>();
            }

            services.AddSingleton<IAuctionRecordManager, AuctionRecordManager>();

            services.AddSingleton<IProductPageLinkHandler, ProductPageLinkHandler>();

            this.AddQuartzServices(services);

            services.AddSingleton<IProductPageLinkHandlerFactory, ProductPageLinkHanlerFactory>();
            services.AddSingleton<WebScraper>();
        });

    private void AddQuartzServices(IServiceCollection services)
    {
        // Add the required Quartz.NET services
        services.AddQuartz(q =>
        {
            // Register the jobs
            q.AddJob<AuctionEndingUpdateJob>(AuctionEndingUpdateJob.Key, c => { c.StoreDurably(true); }); // It has no trigger directly attached.
            q.AddJob<CheckAuctionListsJob>(CheckAuctionListsJob.Key);

            // Create a trigger for the main scraping job
            q.AddTrigger(opts => opts
                .ForJob(CheckAuctionListsJob.Key) // link to the main scraping job
                .WithIdentity($"{CheckAuctionListsJob.Key}-trigger")
                .WithSimpleSchedule(x => x
                    //.WithInterval(this.config.ScrapePeriod) // TODO: put here
                    .WithInterval(TimeSpan.FromMinutes(5))
                    .RepeatForever()));
        });

        // Add the Quartz.NET hosted service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    }
}
