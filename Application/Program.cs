using Application;
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

var webScraperConfig = ConfigurationGuide.GetConfiguration();
