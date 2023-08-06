using System.Threading.Tasks.Dataflow;
using Downloader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductListCrawler;
using WebScraper.Scraping;

namespace WebScraper;

class WebScraper
{
    private ILogger logger;
    private IProductListCrawler crawler;

    public WebScraper(ILogger<WebScraper> logger, IProductListCrawler crawler)
    {
        this.logger = logger;
        this.crawler = crawler;
    }

    public void Run()
    {
        ActionBlock<IReadOnlyCollection<Uri>> action = new(uris =>
        {
            foreach (var uri in uris)
            {
                this.logger.LogInformation(uri.ToString());
            }
        });

        this.crawler.Crawl(new Uri("https://www.auktiva.cz/Keramika-c12035/"), action).Wait();
    }
}

/// <summary>
/// Encapsulates the main entry point to the application.
/// </summary>
internal class Program
{
    /// <summary>
    /// The main entry point to the application.
    /// </summary>
    public static void Main()
    {
        var services = new ServiceCollection();
        services.AddLogging(configure => configure.AddConsole());
        services.AddSingleton<IProductListProcessor, AuktivaProductListProcessor>();
        services.AddSingleton<IHtmlDownloader, Downloader.HtmlDownloader>();
        services.AddSingleton<IProductListCrawler, ProductListCrawler.ProductListCrawler>();
        services.AddSingleton<WebScraper>();


        var provider = services.BuildServiceProvider();
        var app = provider.GetService<WebScraper>();
        app?.Run();

        provider.Dispose();
    }
}