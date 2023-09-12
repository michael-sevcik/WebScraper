using Application.Parsing;
using Downloader;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductListCrawler;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Testcontainers.MsSql;
using WebScraper;
using WebScraper.Configuration;
using WebScraper.Scraping;

namespace WebScraperTests;

public class WebScraperTests
{
    IHtmlDownloader downloader;
    WebScraperConfiguration webScraperConfiguration;
    IAsyncDisposable toDispose;


    [SetUp]
    public async Task Setup()
    {
        var msSqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
            .WithPassword("password")
            .WithName("name") // TODO: Is this the user setup?
            .Build();

        await msSqlContainer.StartAsync();

        this.toDispose = msSqlContainer;
        msSqlContainer.GetConnectionString();

        // Prepare mock downloader and its data
        var pagesByUri = JsonSerializer.Deserialize<Dictionary<string, string>>(TestResources.webPages)
            ?? throw new("Test resource could not be parsed.");

        this.downloader = new MockHtmlDownloader(pagesByUri);

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
            // TODO: Use containerized method
            msSqlContainer.GetConnectionString())
        {
            ScrapePeriod = TimeSpan.FromMinutes(1),
            StoragePeriod = TimeSpan.FromDays(25),
        };
    }

    [TearDown]
    public async Task TearDown()
    {
        await this.toDispose.DisposeAsync();   
    }

    [Test]
    public async Task Test() 
    {
        await this.downloader.GetPageDocumentAsync(new("https://www.auktiva.cz/Rucni-prace-c12130/"));
    }

    [Test]
    public async Task TestScraping()
    {
        // Create and configure the application host builder
        var hostBuilder = Host.CreateDefaultBuilder();

        // Create the web scraper startup instance
        Startup startup = new(this.webScraperConfiguration);
        startup.ConfigureHostBuilder(hostBuilder);

        hostBuilder.ConfigureServices(services =>
        {
            services.AddLogging(x => x.AddConsole());
            services.Replace(ServiceDescriptor.Singleton(this.downloader));
        });


        // Start the application host and wait for a shutdown
        var host = await hostBuilder.StartAsync();
        await host.WaitForShutdownAsync();

        // TODO: Assert data and update changes


    }
}