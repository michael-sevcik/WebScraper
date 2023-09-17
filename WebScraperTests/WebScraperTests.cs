using Application.Parsing;
using Downloader;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Data;
using System.Text.Json;
using Testcontainers.MsSql;
using WebScraper;
using WebScraper.Configuration;
using WebScraper.JobScheduling;
using WebScraper.Persistence;
using WebScraper.Scraping;
using WebScraper.Utils;
using WebScraperTests.Mocks;

namespace WebScraperTests;

public class WebScraperTests
{
    // Start time of DateTimeProvider - 2023/09/10 17:31
    private static readonly DateTime StartTime = new(2023, 09, 10, 17, 31, 0);

    IDateTimeProvider dateTimeProvider;
    IHtmlDownloader mockDownloader;
    WebScraperConfiguration webScraperConfiguration;
    IAsyncDisposable toDispose;
    string dbConnectionString;


    [SetUp]
    public async Task Setup()
    {
        // Create a mock date time provider and its methods in setting the testing quartz methods
        this.dateTimeProvider = new MockDateTimeProvider(StartTime);
        SystemTime.Now = () => this.dateTimeProvider.Now;
        SystemTime.UtcNow = () => this.dateTimeProvider.UtcNow;

        // Create a test MSSQL server container for the web scraper
        var msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
            .Build();

        await msSqlContainer.StartAsync();
        this.dbConnectionString = msSqlContainer.GetConnectionString();

        // Set the test container to toDispose field for disposing by test tear down
        this.toDispose = msSqlContainer;

        // Prepare mock mockDownloader and its data
        var pagesByUri = JsonSerializer.Deserialize<Dictionary<string, string>>(TestResources.webPages)
            ?? throw new("Test resource could not be parsed.");

        this.mockDownloader = new MockHtmlDownloader(pagesByUri);

        // WebScraper configuration
        ProductListProcessorConfiguration listProcessorConfig = new(
            "tbody .name a",
            ".counter a.selected",
            NextPageSelectionType.nextElementToCurrenctlySelected);

        ProductListProcessor productListProcessor = new(listProcessorConfig);

        ProductPageProcessorConfiguration pageProcessorConfiguration = new(
            new("#auctionInfo div div.clear", "dd. MM. yyyy. HH:mm"),
            "#auctioninfobox.box.b div.body div h2",
            "div.price",
            ".left \u003E h1:nth-child(1)",
            new NameSelectorPair[] { new("Location", ".zip") });
        ProductPageProcessor productPageProcessor = new(pageProcessorConfiguration);

        ScrapingJobDefinition scrapingJob = new(
            new Uri[] { new("https://www.auktiva.cz/Rucni-prace-c12130/") },
            productListProcessor,
            productPageProcessor);

        this.webScraperConfiguration = new(
            new ScrapingJobDefinition[] { scrapingJob },
            this.dbConnectionString)
        {
            ScrapePeriod = TimeSpan.FromMinutes(1),
            StoragePeriod = TimeSpan.FromDays(25),
        };
    }

    [TearDown]
    public async Task TearDown()
    {
        if (this.toDispose is not null)
        {
            await this.toDispose.DisposeAsync();
        }
    }

    /// <summary>
    /// Creates a historical record in the test container and starts the quartz.net hosted service.
    /// The delete job should delete the historical record and the check auction list job should start the web scraping.
    /// In test container database should be only the newly scraped records and the historical should be deleted.
    /// </summary>
    /// <returns>A task object representing the asynchronous operation.</returns>
    [Test]
    public async Task TestScraping()
    {
        // Create and configure the application host builder
        var hostBuilder = Host.CreateDefaultBuilder();

        // Create the web scraper startup instance -
        Startup startup = new(this.webScraperConfiguration);
        startup.ConfigureHostBuilder(hostBuilder);

        // Replace some services for testing purposes.
        MockJobScheduler jobScheduler = new();
        hostBuilder.ConfigureServices(services =>
        {
            services.AddLogging(x => x.AddConsole());
            services.Replace(ServiceDescriptor.Singleton(this.mockDownloader));
            services.Replace(ServiceDescriptor.Singleton<IDateTimeProvider>(new MockDateTimeProvider(StartTime)));
            services.Replace(ServiceDescriptor.Singleton<IJobScheduler>(jobScheduler));

            using var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ScraperDbContext>() ??
                throw new("ScraperDbContext cannot be instantiated.");

            // Create a historical auction record that should be deleted by the DeleteOldRecordsJob
            _ = context.AuctionRecords.Add(new()
            {
                EndOfAuction = dateTimeProvider.Now - TimeSpan.FromDays(40),
                Name = "To delete record",
                UniqueIdentifier = "To delete record",
            });

            context.SaveChanges();
        });

        // Start the application host and wait for a shutdown
        using var host = await hostBuilder.StartAsync();

        await Task.Delay(5000);
        await host.StopAsync();

        // Prepare the test result data
        ScraperDbContext context = new();
        context.Database.SetConnectionString(this.dbConnectionString);
        if (!await context.Database.CanConnectAsync())
        {
            throw new Exception("Database connection failed.");
        }

        var records = context.AuctionRecords.OrderBy(x => x.UniqueIdentifier).ToArray();
        var serializedAuctionRecords = JsonSerializer.Serialize(records);

        var serializedScheduledJobDetails = JsonSerializer.Serialize(jobScheduler.ScheduledJobs);
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(serializedAuctionRecords, Is.EqualTo(TestResources.DbScraperExpectedResult));
            Assert.That(serializedScheduledJobDetails, Is.EqualTo(TestResources.ExpectedDetailsOfScheduledJobs));
        });
    }

    /// <summary>
    /// Creates a test auction record in database that represents a auction that ended 10 days ago.
    /// During scraping this record should be matched with a new record and a notification should be sent.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task TestNotifying()
    {
        // Create and configure the application host builder
        var hostBuilder = Host.CreateDefaultBuilder();

        // Prepare mock notifier for counting notifications
        MockNotifier notifier = new();

        // Create the web scraper startup instance -
        Startup startup = new(this.webScraperConfiguration, notifier);
        startup.ConfigureHostBuilder(hostBuilder);

        // Replace some services for testing purposes.
        hostBuilder.ConfigureServices(services =>
        {
            services.AddLogging(x => x.AddConsole());
            services.Replace(ServiceDescriptor.Singleton(this.mockDownloader));
            services.Replace(ServiceDescriptor.Singleton<IDateTimeProvider>(new MockDateTimeProvider(StartTime)));

            using var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<ScraperDbContext>() ??
                throw new("ScraperDbContext cannot be instantiated.");

            // Create a historical auction record that should be matched with a new auction record
            _ = context.AuctionRecords.Add(new()
            {
                EndOfAuction = dateTimeProvider.Now - TimeSpan.FromDays(10),
                Name = "Record of an ended auction",
                UniqueIdentifier = "Palièkovaný obrázek dámy s kloboukem *333",
            });

            context.SaveChanges();
        });

        // Start the application host and wait for a shutdown
        using var host = await hostBuilder.StartAsync();

        await Task.Delay(5000);
        await host.StopAsync();

        // Assert
        Assert.That(notifier.NotificationCount, Is.EqualTo(1));
    }
}