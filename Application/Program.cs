using Application;
using HtmlAgilityPack;
using MailSender;
using Microsoft.EntityFrameworkCore.SqlServer.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Serilog;
using Spectre.Console;
using System.Security.Policy;
using WebScraper;
using WebScraper.Notifications;

AnsiConsole.Write(
    new FigletText("Auction Web Scraper")
        .LeftJustified()
        .Color(Color.Teal));

// Get the user defined configuration
var applicationConfiguration = ConfigurationGuide.GetConfiguration();
var emailNotificationConfiguration = applicationConfiguration.EmailNotificationConfiguration;

// Create a email notifier
EmailNotifier emailNotifier = new(
    new MailKitSender(emailNotificationConfiguration.SmtpConfiguration),
    emailNotificationConfiguration.Recipient);

// Create and configure the application host builder
var hostBuilder = Host.CreateDefaultBuilder();
hostBuilder.ConfigureServices(services =>
{
    //services.AddSerilog((config) =>
    //{
    //    config.WriteTo.Console();
    //    //config.WriteTo.File("logs/log.txt");
    //});
    //services.AddLog4Net();
    //services.AddLogging(configure => configure.AddConsole());
});
hostBuilder.ConfigureLogging(logging =>
{
    //logging.ClearProviders();
    logging.AddLog4Net();
    //logging.AddConsole();
});

// Create the web scraper startup instance
Startup startup = new(applicationConfiguration.WebScraperConfiguration, emailNotifier);
startup.ConfigureHostBuilder(hostBuilder);

// Start the application host and wait for a shutdown
var host = await hostBuilder.StartAsync();
await host.WaitForShutdownAsync();


