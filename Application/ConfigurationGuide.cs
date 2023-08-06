using Application.Configuration;
using Application.Parsing;
using Downloader;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Spectre.Console;
using System.Text.Json;
using System.Xml.XPath;
using WebScraper.Configuration;

namespace Application;

internal static class ConfigurationGuide
{
    public static WebScraperConfig GetConfiguration()
    {
        var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AuctionWebScraper");
        System.IO.Directory.CreateDirectory(folderPath);
        var configurationFilePath = Path.Combine(folderPath, "applicationConfiguration.json");

        ApplicationConfiguration? appConfig = null;
        if (File.Exists(configurationFilePath))
        {
            var jsonText = File.ReadAllText(configurationFilePath);
            appConfig = JsonSerializer.Deserialize<ApplicationConfiguration>(jsonText);
        }

        if (appConfig is null)
        {
            appConfig = RunConfigurationGuide();
            File.WriteAllText(configurationFilePath, JsonSerializer.Serialize(appConfig));
        }

        return appConfig.BuildWebScraperConfiguration();
    }

    private static ApplicationConfiguration RunConfigurationGuide()
    {
        List<SerializableScrapingJobDefinition> scrapingJobs = new();
        do
        {
            scrapingJobs.Add(GetScrapingJob());
        } while (AnsiConsole.Confirm("Do you want to add another scraping job?"));

        return new ApplicationConfiguration(scrapingJobs)
        {
            ScrapePeriod = TimeSpan.FromSeconds(AnsiConsole.Prompt(new TextPrompt<int>("Enter a scraping period in minutes:")
                .Validate(period => period > 0, "The value must be positive."))),
            StoragePeriod = TimeSpan.FromDays(AnsiConsole.Prompt(new TextPrompt<int>("Enter a storage period (for how long should be the ended auction records stored):")
                .Validate(period => period > 0, "The value must be positive."))),
        };
    }

    private static SerializableScrapingJobDefinition GetScrapingJob()
    {
        var uris = GetProductListUris();
        
        // Download the first product list page
        Uri firstProductListPageUri = uris.First();
        HtmlDownloader downloader = new(); // TODO: Consider moving somewhere else or utilizing DI.
        var htmlDocTask = downloader.GetPageDocumentAsync(firstProductListPageUri);
        ConsoleHelper.WaitForTask(() => htmlDocTask.Wait(), "Downloading a product list page...");

        // Get the product list processor configuration
        var documentNode = htmlDocTask.Result.DocumentNode;
        var (productListProcessorConfiguration, productPageExapleLink) = GetListProcessorConfigurationAndProductPageLink(documentNode);

        var productPageUri = CombineUris(firstProductListPageUri, productPageExapleLink);
        if (productPageUri is null)
        {
            throw new(); // todo:
        }

        // Download a product page
        htmlDocTask = downloader.GetPageDocumentAsync(productPageUri);
        ConsoleHelper.WaitForTask(() => htmlDocTask.Wait(), "Downloading a product page..."); // TODO: handle errors

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
        string cssProductPageSelector = null!; // initialized by the lower lambda function.
        string? exampleProductPageLink = null;
        DoUntil(true, exampleLink => $"Is this \"{exampleLink}\" a correct link (relative or absolute) to a product page", () =>
        {
            cssProductPageSelector = AnsiConsole.Prompt(new TextPrompt<string>("CSS selector for selecting the links:")); // TODO: Add validation

            // Wait for htmlDoc downloading and parsing
            var node = documentNode.QuerySelector(cssProductPageSelector);
            if (node is null)
            {
                return "Element could not been found.";
            }

            exampleProductPageLink = ProductListProcessor.ParseLink(node);
            return exampleProductPageLink;
        });

        // to select next page we will let user pick between selecting next page button and selecting next page from page list box.
        var nextPageSelectionType = AnsiConsole.Prompt(new SelectionPrompt<NextPageSelectionType>()
            .AddChoices(NextPageSelectionType.nextElementFromCurrenctlySelected, NextPageSelectionType.button));

        string nextPageCssSelector = null!; // This is always filled by the lambda function
        DoUntil(true, exampleLink => $"Is this \"{exampleLink}\" a correct link (relative or absolute) to the 2nd product list page?", () =>
        {
            nextPageCssSelector = AnsiConsole.Prompt(new TextPrompt<string>("CSS selector for next page element:")); // TODO: Add validation
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
        string endOfAuctionXPathSelector = GetXPath(documentNode, "the auction end date");
        string uniqueIdentificationXPathSelector = GetXPath(
            documentNode,
            "a unique identifier for storing the result and identifying readded items.");

        string priceXPathSelector = GetXPath(documentNode, "the current price of the item");
        string nameXPathSelector = GetXPath(documentNode, "the auction name");

        // Add additional info
        List<NameSelectorPair> nameSelectorPairs = new();
        while (AnsiConsole.Confirm("Do you want to add any additional information?"))
        {
            // get values from user
            string name = AnsiConsole.Ask<string>("Name idetifying the infromation:");
            string xPathSelector = GetXPath(documentNode, name);

            // and another pair
            nameSelectorPairs.Add(new(name, xPathSelector));
        }

        return new(endOfAuctionXPathSelector,
            uniqueIdentificationXPathSelector,
            priceXPathSelector,
            nameXPathSelector,
            nameSelectorPairs);
    }

    private static string GetXPath(HtmlNode documentNode, string nameOfSelectedInformation)
    {
        string xpath = null!;
        DoUntil(true, value => $"Is this \"{value}\" the correct value for {nameOfSelectedInformation}?", () =>
        {
            xpath = AnsiConsole.Prompt(
                new TextPrompt<string>($"XPath selector for selecting {nameOfSelectedInformation}:")
                .Validate(xpath =>
                {
                    try
                    {
                        XPathExpression.Compile(xpath);
                        var node = documentNode.SelectSingleNode(xpath);
                        _ = node.InnerHtml; // TODO: consider checking for null
                    }
                    catch (Exception ex)
                    {
                        return ValidationResult.Error(ex.Message);
                    }

                    return ValidationResult.Success();
                }));

            return documentNode.SelectSingleNode(xpath).InnerText; // TODO: what happens when there is no element with matching xpath.
        });

        return xpath;
    }

    private static Uri? CombineUris(Uri baseUri, string? rest) // TODO: Consider moving to new project utils.
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
