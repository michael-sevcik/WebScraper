
using Spectre.Console;
using System.Reflection.Metadata.Ecma335;
using WebScraper.Configuration;

AnsiConsole.Write(
    new FigletText("Auction Web Scraper")
		.LeftJustified()
        .Color(Color.Teal));

if (AnsiConsole.Confirm("Do you want to adjust scraping configuration?")) // TODO: run configuration implicitly when config file does not exist.
{
    AppConfig? appConfig = new(); // TODO: load if already exists.
    RunConfigurationGuide(ref appConfig);
}

AnsiConsole.Status()
    .Start("Thinking...", ctx =>
    {
        // Simulate some work
        AnsiConsole.MarkupLine("Doing some work...");
        Thread.Sleep(1000);

        // Update the status and spinner
        ctx.Status("Thinking some more");
        ctx.Spinner(Spinner.Known.Star);
        ctx.SpinnerStyle(Style.Parse("green"));

        // Simulate some work
        AnsiConsole.MarkupLine("Doing some more work...");
        Thread.Sleep(10000);
    });

void RunConfigurationGuide(ref AppConfig? appConfig)
{
    if (appConfig is null)
    {
        appConfig = new();

    }

    appConfig.ScrapePeriod = TimeSpan.FromSeconds(AnsiConsole.Ask<uint>("Screpe period in seconds: ", (uint)appConfig.ScrapePeriod.TotalSeconds));

    AnsiConsole.WriteLine(appConfig.ScrapePeriod.ToString());
}

void RunScrapingSetup(ref AppConfig appConfig)
{
    // TODO:
}

  
