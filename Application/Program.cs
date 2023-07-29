
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

//AnsiConsole.Status()
//    .Start("Thinking...", ctx =>
//    {
//        // Simulate some work
//        AnsiConsole.MarkupLine("Doing some work...");
//        Thread.Sleep(1000);

//        // Update the status and spinner
//        ctx.Status("Thinking some more");
//        ctx.Spinner(Spinner.Known.Star);
//        ctx.SpinnerStyle(Style.Parse("green"));

//        // Simulate some work
//        AnsiConsole.MarkupLine("Doing some more work...");
//        Thread.Sleep(10000);
//    });

void RunConfigurationGuide(ref AppConfig? appConfig)
{
    if (appConfig is null)
    {
        appConfig = new();

    }

    appConfig.ScrapePeriod = TimeSpan.FromSeconds(AnsiConsole.Ask<uint>("Screpe period in seconds: ", (uint)appConfig.ScrapePeriod.TotalSeconds));

    AnsiConsole.WriteLine(appConfig.ScrapePeriod.ToString());
    RunScrapingSetup(ref appConfig);
}

void RunScrapingSetup(ref AppConfig appConfig)
{
    ScrapingJobDefinition GetScrapingJob()
    {
        var stringURI = AnsiConsole.Prompt(new TextPrompt<string>("Uri of the first product list page: ")
            .Validate(str => Uri.IsWellFormedUriString(str, UriKind.Absolute), "The URI is not well formated.")
            );
        Uri firstProductListPageUri = new(stringURI);

        var cssProductPageSelector = AnsiConsole.Prompt(new TextPrompt<string>("CSS selector for selecting the links.")); // TODO: Add validation
        ParseProductPageLinksDelegate productPageLinkParser = node =>
        {
            var productRecords = node.QuerySelectorAll(cssProductPageSelector);
            var links = from productRecord in productRecords
                        select new Uri(productRecord
                            .SelectSingleNode(".//a[@href]")
                            .GetAttributeValue("href", string.Empty));
            return links.ToList();
        };

        // to select next page we will let user pick between selecting next page button and selecting next page from page list box.
        ParseNextPageLinkDelegate nextPageLinkParser = x => string.Empty; // TODO:
        switch (AnsiConsole.Prompt(new SelectionPrompt<NextPageSelection>().AddChoices(NextPageSelection.nextElementFromCurrenctlySelected, NextPageSelection.button))) // TODO: Question text
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

enum NextPageSelection
{
    button,
    nextElementFromCurrenctlySelected,
}
