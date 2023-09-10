using Application.Configuration;
using Application.Parsing;
using Application.Utilities;
using Downloader;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using MailKit.Net.Smtp;
using MailSender;
using Microsoft.Data.SqlClient;
using MimeKit;
using Spectre.Console;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Xml.XPath;
using WebScraper.Configuration;

namespace Application;

internal static class ConfigurationGuide
{
    private static readonly string ConfigurationDirectoryName = "AuctionWebScraper";
    private static readonly string SerializedWebScraperFileName = "WebScraperConfiguration.json";
    private static readonly string SerializedEmailNotificationConfiguration = "EmailNotificationConfiguration.json";

    public static ApplicationConfiguration GetConfiguration()
    {
        var webscraperConfiguration = GetWebScraperConfiguration();
        var emailConfiguration = GetEmailConfiguration();

        return new ApplicationConfiguration(emailConfiguration, webscraperConfiguration);
    }

    private static WebScraperConfiguration GetWebScraperConfiguration()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationDirectoryName);
        System.IO.Directory.CreateDirectory(folderPath);
        var configurationFilePath = Path.Combine(folderPath, SerializedWebScraperFileName);

        SerializableWebScraperConfiguration? configuration = null;
        if (File.Exists(configurationFilePath))
        {
            var jsonText = File.ReadAllText(configurationFilePath);
            configuration = JsonSerializer.Deserialize<SerializableWebScraperConfiguration>(jsonText);
        }

        if (configuration is null)
        {
            configuration = RunWebScraperConfigurationGuide();
            File.WriteAllText(configurationFilePath, JsonSerializer.Serialize(configuration));
        }

        return configuration.BuildWebScraperConfiguration();
    }

    private static EmailNotificationConfiguration GetEmailConfiguration()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ConfigurationDirectoryName);
        System.IO.Directory.CreateDirectory(folderPath);
        var configurationFilePath = Path.Combine(folderPath, SerializedEmailNotificationConfiguration);

        EmailNotificationConfiguration? configuration = null;
        if (File.Exists(configurationFilePath))
        {
            var jsonText = File.ReadAllText(configurationFilePath);
            configuration = JsonSerializer.Deserialize<EmailNotificationConfiguration>(jsonText);
        }

        if (configuration is null)
        {
            configuration = RunEmailConfigurationGuide();
            File.WriteAllText(configurationFilePath, JsonSerializer.Serialize(configuration));
        }

        return configuration;
    }

    private static EmailNotificationConfiguration RunEmailConfigurationGuide()
    {
        using SmtpClient smtpClient = new();

        var tryConnectingToServer = (string host, int port) =>
        {
            try
            {
                ConsoleHelper.WaitForTask(() => smtpClient.Connect(host, port), "Trying to connect to the server.");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Connection failed due to: {ex.Message}");
                return false;
            }

            return true;
        };

        AnsiConsole.Write(new FigletText("Email Notifications Configuration Guide")
            .LeftJustified()
            .Color(Color.Teal));

        // Get the server address from user
        string host;
        int port;
        do
        {
            host = AnsiConsole.Ask<string>("SMTP host address:");
            port = AnsiConsole.Ask<int>("SMTP port: ");
        } while (!tryConnectingToServer(host, port));

        // disconnect for later authentication test connection
        smtpClient.Disconnect(true);

        var tryAuthenticating = (string username, string password) =>
        {
            try
            {
                ConsoleHelper.WaitForTask(() => smtpClient.Connect(host, port), "Trying to connect to the server.");
                ConsoleHelper.WaitForTask(() => smtpClient.Authenticate(username, password), "Trying to authenticate to the server.");
                ConsoleHelper.WaitForTask(() => smtpClient.Disconnect(true), "Trying to authenticate to the server.");
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Authenticating failed due to: {ex.Message}");
                return false;
            }

            return true;
        };

        // Get the User credentials
        string userName;
        string password;
        do
        {
            userName = AnsiConsole.Ask<string>("Username:");
            password = AnsiConsole.Ask<string>("password:");
        } while (!tryAuthenticating(userName, password));

        var senderAddress = AnsiConsole.Prompt(new TextPrompt<string>("Mailbox address of the notification sender:")
            .DefaultValue(userName)
            .Validate(a => MailboxAddress.TryParse(a, out var _), "The mail address is incorrect."));

        // Get the recipient address
        var recipient = AnsiConsole.Prompt(new TextPrompt<string>("Mailbox address of the notification recipient:")
            .Validate( a => MailboxAddress.TryParse(a, out var _), "The mail address is incorrect."));

        return new(new(userName, password, host, port, senderAddress), recipient);
    }

    private static SerializableWebScraperConfiguration RunWebScraperConfigurationGuide()
    {
        AnsiConsole.Write(new FigletText("Web Scraper Configuration Guide")
            .LeftJustified()
            .Color(Color.Teal));

        // Get a valid connection string
        var dbConnectionString = AnsiConsole.Prompt(new TextPrompt<string>("Enter a MSSQL Server database connection string:\n").Validate(cs =>
        {
            try
            {
                var factory = SqlClientFactory.Instance;
                using var connection = factory.CreateConnection() ?? throw new Exception();
                connection.ConnectionString = cs;
                connection.Open();
            }
            catch (Exception ex)
            {
                return ValidationResult.Error($"Db Connection using the entered connection string could not be established. {ex.Message}");
            }

            return ValidationResult.Success();
        }));

        // Get the scraping job definitions
        List<SerializableScrapingJobDefinition> scrapingJobs = new();
        do
        {
            scrapingJobs.Add(GetScrapingJob());
        } while (AnsiConsole.Confirm("Do you want to add another scraping job?"));

        // Get the last configurations and create a configured application configuration instance
        return new SerializableWebScraperConfiguration(scrapingJobs)
        {
            ScrapePeriod = TimeSpan.FromSeconds(AnsiConsole.Prompt(new TextPrompt<int>("Enter a scraping period in minutes:")
                .Validate(period => period > 0, "The value must be positive."))),
            StoragePeriod = TimeSpan.FromDays(AnsiConsole.Prompt(new TextPrompt<int>("Enter a storage period (for how many days should be the ended auction records stored):")
                .Validate(period => period > 0, "The value must be positive."))),
            DbConnectionString = dbConnectionString,
        };
    }

    private static SerializableScrapingJobDefinition GetScrapingJob()
    {
        var uris = GetProductListUris();
        
        // Download the first product list page
        Uri firstProductListPageUri = uris.First();
        HtmlDownloader downloader = new();
        var htmlDocTask = downloader.GetPageDocumentAsync(firstProductListPageUri);
        ConsoleHelper.WaitForTask(() => htmlDocTask.Wait(), "Downloading a product list page...");

        // Get the product list processor configuration
        var documentNode = htmlDocTask.Result.DocumentNode;
        var (productListProcessorConfiguration, productPageExapleLink) = GetListProcessorConfigurationAndProductPageLink(documentNode);

        var productPageUri = CombineUris(firstProductListPageUri, productPageExapleLink)
            ?? throw new($"An error occurred while combining the base URI and link to the product page: \"{firstProductListPageUri}\" and \"{productPageExapleLink}\"");

        // Download a product page
        htmlDocTask = downloader.GetPageDocumentAsync(productPageUri);
        ConsoleHelper.WaitForTask(() => htmlDocTask.Wait(), "Downloading a product page...");

        // Get the product page processor configuration
        documentNode = htmlDocTask.Result.DocumentNode;
        var productPagePricessorConfiguration = GetProductPageProcessorConfiguration(documentNode);

        return new(uris, productListProcessorConfiguration, productPagePricessorConfiguration);
    }

    private static List<Uri> GetProductListUris()
    {
        List<Uri> result = new();

        // Add all URIs to scrape that user wants.
        do
        {
            Uri uri = new(AnsiConsole.Prompt(new TextPrompt<string>("Uri of the first product list page: ")
                .Validate(
                    str => Uri.IsWellFormedUriString(str, UriKind.Absolute),
                    "The URI is not well formated.")));

            result.Add(uri);
        } while (AnsiConsole.Confirm("Do you want to add another product list link to this scraping profile?"));

        return result;
    }

    private static (ProductListProcessorConfiguration, string) GetListProcessorConfigurationAndProductPageLink(HtmlNode documentNode)
    {
        string cssProductPageSelector = null!; // initialized by the lambda function below.
        string? exampleProductPageLink = null;
        DoUntil(true, exampleLink => $"Is this \"{exampleLink}\" a correct link (relative or absolute) to a product page", () =>
        {
            cssProductPageSelector = AnsiConsole.Prompt(new TextPrompt<string>("CSS selector for selecting the links:"));

            // Wait for htmlDoc downloading and parsing
            var node = documentNode.QuerySelector(cssProductPageSelector);
            if (node is null)
            {
                return $"Element could not been found using the \"{cssProductPageSelector}\" CSS selector.";
            }

            exampleProductPageLink = ProductListProcessor.ParseLink(node);
            return exampleProductPageLink;
        });

        // to select next page we will let user pick between selecting next page button and selecting next page from page list box.
        var nextPageSelectionType = AnsiConsole.Prompt(new SelectionPrompt<NextPageSelectionType>()
            .AddChoices(NextPageSelectionType.nextElementToCurrenctlySelected, NextPageSelectionType.button));

        string nextPageCssSelector = null!; // This is always filled by the lambda function
        DoUntil(true, exampleLink => $"Is this \"{exampleLink}\" a correct link (relative or absolute) to the 2nd product list page?", () =>
        {
            nextPageCssSelector = AnsiConsole.Prompt(new TextPrompt<string>("CSS selector for next page element:"));
            var exampleLink = ProductListProcessor.ParseNextPage(documentNode, nextPageSelectionType, cssProductPageSelector);

            var node = documentNode.QuerySelector(nextPageCssSelector);
            exampleLink = ProductListProcessor.ParseLink(node);
            if (exampleLink is null)
            {
                AnsiConsole.WriteLine($"The link was not found; this is the node that was selected: \n{node.OuterHtml}");
                return null;
            }

            return exampleLink;
        });

        // The example link cannot be null here, because it is filled during execution of the above DoUntil method.
        if (exampleProductPageLink is null)
        {
            throw new Exception("An internal error occured due to unxpected circumstances");
        }
        return (new(cssProductPageSelector, nextPageCssSelector, nextPageSelectionType), exampleProductPageLink);
    }

    private static ProductPageProcessorConfiguration GetProductPageProcessorConfiguration(HtmlNode documentNode)
    {
        // Add predefined information

        var endOfAuctionSelector = new DateWithExactFormatSelector(
            GetCssSelector(documentNode, "the auction end date"),
            AnsiConsole.Ask<string>("Enter the exact datetime format for parsing (e. g. dd/MM/yyyy HH:mm:ss)"));

        string uniqueIdentificationSelector = GetCssSelector(
            documentNode,
            "a unique identifier for storing the result and identifying readded items.");

        string priceSelector = GetCssSelector(documentNode, "the current price of the item");
        string nameSelector = GetCssSelector(documentNode, "the auction name");

        // Add additional info
        List<NameSelectorPair> nameSelectorPairs = new();
        while (AnsiConsole.Confirm("Do you want to add any additional information?"))
        {
            // get values from user
            string name = AnsiConsole.Ask<string>("Name idetifying the infromation:");
            string xPathSelector = GetCssSelector(documentNode, name);

            // and another pair
            nameSelectorPairs.Add(new(name, xPathSelector));
        }

        return new(
            endOfAuctionSelector,
            uniqueIdentificationSelector,
            priceSelector,
            nameSelector,
            nameSelectorPairs);
    }

    private static string GetCssSelector(HtmlNode documentNode, string nameOfSelectedInformation)
    {
        string cssSelector = null!;
        DoUntil(true, value => $"Is this \"{value}\" the correct value for {nameOfSelectedInformation}?", () =>
        {
            cssSelector = AnsiConsole.Prompt(
                new TextPrompt<string>($"CSS selector for selecting {nameOfSelectedInformation}:")
                .Validate(selector =>
                {
                    try
                    {
                        var node = documentNode.QuerySelector(selector);
                        _ = node.InnerHtml;
                    }
                    catch (Exception ex)
                    {
                        return ValidationResult.Error(ex.Message);
                    }

                    return ValidationResult.Success();
                }));

            return documentNode.QuerySelector(cssSelector).InnerText;
        });

        return cssSelector;
    }

    private static Uri? CombineUris(Uri baseUri, string? rest)
    {
        if (Uri.IsWellFormedUriString(rest, UriKind.Absolute))
        {
            return new Uri(rest);
        }

        Uri.TryCreate(baseUri, rest, out var result);
        return result;
    }

    private static void DoUntil<TResult>(
        bool shouldRepeatUntilConfirmation,
        Func<TResult, string> confirmationMessageGetter,
        Func<TResult> toDoUntilConfirmationFunc)
    {
        TResult result;
        do
        {
            result = toDoUntilConfirmationFunc();
        } while (AnsiConsole.Confirm(confirmationMessageGetter(result)) != shouldRepeatUntilConfirmation);
    }
}
