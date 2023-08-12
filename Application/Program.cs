using Application;
using Spectre.Console;

AnsiConsole.Write(
    new FigletText("Auction Web Scraper")
        .LeftJustified()
        .Color(Color.Teal));

var webScraperConfig = ConfigurationGuide.GetConfiguration();
