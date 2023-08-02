using Downloader;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Spectre.Console;
using WebScraper.Configuration;
using WebScraper.Scraping;

AnsiConsole.Write(
    new FigletText("Auction Web Scraper")
        .LeftJustified()
        .Color(Color.Teal));

if (AnsiConsole.Confirm("Do you want to adjust scraping configuration?")) // TODO: run configuration implicitly when config file does not exist.
{
    AppConfig? appConfig = new(); // TODO: load if already exists.
    RunConfigurationGuide(ref appConfig);
}

void RunConfigurationGuide(ref AppConfig? appConfig)
{
    appConfig ??= new();

    appConfig.ScrapePeriod = TimeSpan.FromSeconds(AnsiConsole.Ask<uint>("Screpe period in seconds: ", (uint)appConfig.ScrapePeriod.TotalSeconds));

    AnsiConsole.WriteLine(appConfig.ScrapePeriod.ToString());
    RunScrapingSetup(ref appConfig);
}

void RunScrapingSetup(ref AppConfig appConfig)
{
    ScrapingJobDefinition GetScrapingJob()
    {
        var parseLink = static (HtmlNode node) => node
            .SelectSingleNode(".//a[@href]")
            .GetAttributeValue("href", string.Empty);

        var stringURI = AnsiConsole.Prompt(new TextPrompt<string>("Uri of the first product list page: ")
            .Validate(str => Uri.IsWellFormedUriString(str, UriKind.Absolute), "The URI is not well formated.")
            );
        Uri firstProductListPageUri = new(stringURI);

        HtmlDownloader downloader = new();
        var htmldoc = downloader.GetPageDocumentAsync(firstProductListPageUri);

        string cssProductPageSelector;
        {
            string exampleLink;
            do
            {
                cssProductPageSelector = AnsiConsole.Prompt(new TextPrompt<string>("CSS selector for selecting the links:")); // TODO: Add validation

                // Wait for htmlDoc downloading and parsing
                WaitForTask(() => htmldoc.Wait(), "Preparing the link...");
                var node = htmldoc.Result.QuerySelector(cssProductPageSelector);
                exampleLink = parseLink(node);
            }
            while (!AnsiConsole.Confirm($"Is this \"{exampleLink}\" a correct link to a product page"));
        }

        ParseProductPageLinksDelegate productPageLinkParser = node =>
        {
            var productRecords = node.QuerySelectorAll(cssProductPageSelector);
            var links = from productRecord in productRecords
                        select new Uri(parseLink(productRecord));
            return links.ToList();
        };

        // to select next page we will let user pick between selecting next page button and selecting next page from page list box.
        ParseNextPageLinkDelegate nextPageLinkParser = x => string.Empty; // TODO:
        switch (AnsiConsole.Prompt(new SelectionPrompt<NextPageSelection>()
            .AddChoices(NextPageSelection.nextElementFromCurrenctlySelected, NextPageSelection.button))) // TODO: Question text
        {
            case NextPageSelection.button:
                break;
            case NextPageSelection.nextElementFromCurrenctlySelected:
                break;
            default:
                throw new NotSupportedException();
        }

        

        return new(firstProductListPageUri, productPageLinkParser, nextPageLinkParser);
    }

    var scrapingJobs = appConfig.ScrapingJobs;
    if (!scrapingJobs.Any())
    {
        scrapingJobs.Add(GetScrapingJob());
    }

    while (AnsiConsole.Confirm("Do you want to add another scraping job?"))
    {
        scrapingJobs.Add(GetScrapingJob());
    }
}

void WaitForTask(Action action, string waitMessage)
    => AnsiConsole.Status().Start("Thinking...", ctx =>
    {
        // Set the status and spinner
        ctx.Status(waitMessage);
        ctx.Spinner(Spinner.Known.Star);
        ctx.SpinnerStyle(new Style(Color.Green));

        // Execute the action
        action();
        Thread.Sleep(5000); // TODO: DELETE this.
    });

enum NextPageSelection
{
    button,
    nextElementFromCurrenctlySelected,
}

