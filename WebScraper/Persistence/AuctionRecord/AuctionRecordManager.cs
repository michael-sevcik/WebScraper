using System.Text.Json;
using Microsoft.Extensions.Logging;
using WebScraper.Notifications;
using WebScraper.Scraping;

namespace WebScraper.Persistence.AuctionRecord;

/// <summary>
/// An implementation of the <see cref="IAuctionRecordManager"/> that uses an instance
/// of <see cref="IAuctionRecordRepository"/> to manage the records.
/// </summary>
internal class AuctionRecordManager : IAuctionRecordManager
{
    private readonly ILogger logger;
    private readonly IAuctionRecordRepository recordRepository;
    private readonly INotifier notifier;
    private readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="AuctionRecordManager"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="notifier">Object that handles sending of the notifications.</param>
    /// <param name="recordRepository">A repository that should be used for storing the auction records.</param>
    public AuctionRecordManager(ILogger<AuctionRecordManager> logger, IAuctionRecordRepository recordRepository, INotifier notifier)
    {
        this.logger = logger;
        this.recordRepository = recordRepository;
        this.notifier = notifier;
    }

    /// <inheritdoc/>
    public async Task HandleParsedProductPageAsync(ParsedProductPage parsedProductPage, Uri sourceUri)
    {
        var storedRecord = await this.recordRepository.GetOrDefault(parsedProductPage.UniqueIdentifier);
        if (storedRecord is not null)
        {
            // If the auction item already has an record, check whether the previous auction has ended.
            if (storedRecord.Ended && parsedProductPage.EndOfAuction > storedRecord.EndOfAuction)
            {
                // Send a notification.
                string message = $"""
                    Old auction data:
                    {JsonSerializer.Serialize(parsedProductPage, this.jsonSerializerOptions)}
                    
                    New auction data:
                    
                    {JsonSerializer.Serialize(storedRecord, this.jsonSerializerOptions)}
                    """;

                Notification notification = new(
                    reason: "Readded item",
                    tilte: $"Item with the name: \"{storedRecord.Name}\" and unique identifier: \"{storedRecord.UniqueIdentifier}\" was readded.",
                    message: message);

                await this.notifier.NotifyAsync(notification);

                // Update the stored record.
                await this.UpdateAuctionRecordAsync(storedRecord.Id, parsedProductPage, sourceUri);
            }
        }
        else
        {
            await this.recordRepository.AddAsync(this.CreateAuctionRecord(parsedProductPage, sourceUri));

            // TODO: register update job
        }
    }

    /// <inheritdoc/>
    public async Task UpdateAuctionRecordAsync(Guid id, ParsedProductPage parsedProductPage, Uri sourceUri)
    {
        var updatedRecord = this.CreateAuctionRecord(parsedProductPage, sourceUri, id);
        await this.recordRepository.UpdateAsync(updatedRecord);
    }

    private BaseAuctionRecord CreateAuctionRecord(ParsedProductPage parsingResult, Uri sourceUri, Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            EndOfAuction = parsingResult.EndOfAuction,
            Name = parsingResult.Name,
            Price = parsingResult.Price,
            UniqueIdentifier = parsingResult.UniqueIdentifier,
            AdditionalInfromation = parsingResult.AdditionalInfromation,
            Uri = sourceUri.AbsoluteUri,
        };
}
