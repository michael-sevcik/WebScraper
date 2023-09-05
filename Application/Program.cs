using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using WebScraper;

AnsiConsole.Write(
    new FigletText("Auction Web Scraper")
        .LeftJustified()
        .Color(Color.Teal));

// Get the user defined configuration
var webScraperConfig = ConfigurationGuide.GetConfiguration(); // TODO: get the email configuration.

// Create and configure the application host builder
var hostBuilder = Host.CreateDefaultBuilder();
hostBuilder.ConfigureServices(services =>
{
    services.AddLogging(configure => configure.AddConsole()) ;
    //services.AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Trace)) ;
});

// Create the web scraper startup instance
Startup startup = new(webScraperConfig); // TODO: add emailNotifier
startup.ConfigureHostBuilder(hostBuilder);

// Start the application host and wait for a shutdown
var host = await hostBuilder.StartAsync();
await host.WaitForShutdownAsync();


