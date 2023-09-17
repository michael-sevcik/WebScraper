# Web Scraper

This is a console and .NET based web scraper application for capturing readdition of identical items to auction sites, the auction sites have to be static pages, because this web scraper does not support JS loading yet. It provides a simple, easy to use configuration guide, implemented using [Spectre.Console](https://spectreconsole.net/), furthermore the captured readditions are reported by email notifications. For selecting the relevant information the scraper uses CSS selectors.

## Configuration

After starting the application, you will be greeted with the configuration guide. Enter the requested information.

### Web scraping configuration

Configuration of the web scraping is divided into several steps:

1. Db connection string - only MSSQL is supported at the moment.
   - It is needed for storing item auction records that are used in item readdition checks. 
   - E.g. `Data Source=PROVIDER;Initial Catalog=WebScraper;User ID=USER_NAME;Password=PASSWORD;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;`
2. Scraping jobs - definition of one scraping job:
   1. Enter the links to the item list pages you want to scrape that have the same format
   2. Enter the configuration of the item list processor - CSS selectors for item link selection and next page selection.
   3. Enter the configuration of the item page processor - CSS selectors for selecting an end of auction, price, name,... , unique identifier - something really unique to the specific item, e.g. car VIN.
   4. Enter the scraping period in whole minutes - how often should be the scraping jobs started.
   5. Enter the storage period in days - how long should be the records of ended auctions stored (the records of auctions after their end are stored for item readdition checks).
3. Email configuration
   1. Enter a host address and a port of the SMTP server, this address will be checked.
   2. Enter a username and password for authenticating to the server, the SMTP client will try to connect via SSL, if it is available, and authenticate.

For finding the CSS selectors, we recommend using the [Firefox DevTools](https://firefox-source-docs.mozilla.org/devtools-user/).

The configuration will be serialized and stored in your local application data directory under `AuctionWebScraper` directory.

## Scraping

After the configuration process, a web scraper DB context ensures that the database tables are created and a set of jobs, that represents the web scraper, is scheduled (scrape, update ending auction and delete old records job).

During scraping, when a new auction of item with an identical unique identifier to an already stored record is discovered, an email notification will be sent. 

## Architecture and used technologies

If you are interested in the back-end of this application, or the functionality described above does not suit your needs and you want to reuse some of its infrastructure, see [the program documentation](Docs/ProgramDocumentation.md).   
