
namespace WebScraper;
using ProductListCrawler;
using System.Threading.Tasks.Dataflow;

/// <summary>
/// Encapsulates the main entry point to the application.
/// </summary>
internal class Program
{
    /// <summary>
    /// The main entry point to the application.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        ActionBlock<IReadOnlyCollection<Uri>> action = new(uris =>
        {
            foreach (var uri in uris)
            {
                Console.WriteLine(uri);
            }
        });

        ProductListCrawler crawler = new(new AuktivaProductListProcessor());

        crawler.Crawl(new Uri("https://www.auktiva.cz/Keramika-c12035/"), action).Wait();
    }
}