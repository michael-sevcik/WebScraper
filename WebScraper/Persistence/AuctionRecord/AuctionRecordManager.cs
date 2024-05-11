using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;
using WebScraper.JobScheduling;
using WebScraper.Notifications;
using WebScraper.Scraping;
using WebScraper.Utils;

namespace WebScraper.Persistence.AuctionRecord;

/// <summary>
/// An implementation of the <see cref="IAuctionRecordManager"/> that uses an instance
/// of <see cref="IAuctionRecordRepository"/> to manage the records.
/// </summary>
internal class AuctionRecordManager : IAuctionRecordManager
{
    private readonly ILogger logger;
    private readonly IJobScheduler jobScheduler;
    private readonly IAuctionRecordRepository recordRepository;
    private readonly INotifier notifier;
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true,
    };

    private HashSet<string>? uniqueIdentifiers = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionRecordManager"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="scheduler">Scheduler for scheduling update jobs.</param>
    /// <param name="notifier">Object that handles sending of the notifications.</param>
    /// <param name="recordRepository">A repository that should be used for storing the auction records.</param>
    /// <param name="dateTimeProvider">The provider of date and time.</param>
    public AuctionRecordManager(
        ILogger<AuctionRecordManager> logger,
        IJobScheduler scheduler,
        IAuctionRecordRepository recordRepository,
        INotifier notifier,
        IDateTimeProvider dateTimeProvider)
    {
        this.logger = logger;
        this.jobScheduler = scheduler;
        this.recordRepository = recordRepository;
        this.notifier = notifier;
        this.dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc/>
    public async Task HandleParsedProductPageAsync(
        ParsedProductPage parsedProductPage,
        Uri sourceUri,
        IProductPageProcessor productPageProcessor)
    {
        var ids = await this.uniqueIdentifiersTask;
        if (!ids.Contains(parsedProductPage.UniqueIdentifier))
        {
            var newAuctionRecord = this.CreateAuctionRecord(parsedProductPage, sourceUri);
            await this.recordRepository.AddAsync(newAuctionRecord);

            await this.jobScheduler.ScheduleUpdateJobAsync(
                newAuctionRecord.EndOfAuction,
                sourceUri,
                newAuctionRecord.Id,
                productPageProcessor);
        }
        else
        {
            var storedRecord = await this.recordRepository.GetOrDefault(parsedProductPage.UniqueIdentifier)
                               ?? throw new Exception("Internal error - ID must exist");

            await this.HandleExistingRecord(parsedProductPage, sourceUri, productPageProcessor, storedRecord).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAuctionRecordAsync(Guid id, ParsedProductPage parsedProductPage, Uri sourceUri)
    {
        var updatedRecord = this.CreateAuctionRecord(parsedProductPage, sourceUri, id);

        this.logger.LogTrace("Updating the record with id {id} and URI {sourceUri}", id, sourceUri);
        await this.recordRepository.UpdateAsync(updatedRecord);
    }

    private async Task HandleExistingRecord(
        ParsedProductPage parsedProductPage,
        Uri sourceUri,
        IProductPageProcessor productPageProcessor,
        BaseAuctionRecord storedRecord)
    {
        // If the auction item already has an existing stored record, check whether the previous auction had already ended.
        if (storedRecord.EndOfAuction <= this.dateTimeProvider.Now &&
            parsedProductPage.EndOfAuction > storedRecord.EndOfAuction)
        {
            // Send a notification.
            var message = $"""
                           Old auction data:
                           {JsonSerializer.Serialize(storedRecord, this.jsonSerializerOptions)}

                           New auction data:

                           {JsonSerializer.Serialize(parsedProductPage, this.jsonSerializerOptions)}

                           Link: {sourceUri.OriginalString}
                           """;

            Notification notification = new(
                reason: "Readded item",
                tilte: $"Item with the name: \"{storedRecord.Name}\" and unique identifier: \"{storedRecord.UniqueIdentifier}\" was readded.",
                message: message);

            this.logger.LogInformation("Sending notification: {notification}", notification.Title);

            try
            {
                await this.notifier.NotifyAsync(notification);
            }
            catch (Exception ex)
            {
                this.logger.LogError("Notification failed: {message}", ex.Message);
            }

            // Update the stored record.
            await this.UpdateAuctionRecordAsync(storedRecord.Id, parsedProductPage, sourceUri);

            // Schedule update job for the new record
            await this.jobScheduler.ScheduleUpdateJobAsync(
                parsedProductPage.EndOfAuction,
                sourceUri,
                storedRecord.Id,
                productPageProcessor);
        }
    }

    private BaseAuctionRecord CreateAuctionRecord(ParsedProductPage parsingResult, Uri sourceUri, Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            EndOfAuction = parsingResult.EndOfAuction,
            Created = this.dateTimeProvider.Now,
            Name = parsingResult.Name,
            Price = parsingResult.Price,
            UniqueIdentifier = parsingResult.UniqueIdentifier,
            AdditionalInfromation = parsingResult.AdditionalInfromation,
            Uri = sourceUri.AbsoluteUri,
        };

    private async ValueTask<HashSet<string>> GetUniqueIdentifiersAsync()
        => this.uniqueIdentifiers ??= await this.recordRepository.GetAllUniqueIdentifiersAsync();
}
