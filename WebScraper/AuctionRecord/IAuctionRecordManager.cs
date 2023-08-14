using System;

namespace WebScraper.AuctionRecord;

/// <summary>
/// Represents a class for managing auction records.
/// </summary>
internal interface IAuctionRecordManager
{
    /// <summary>
    /// Handles the <paramref name="record"/> - stores it if it is new, or sends a notification if it was readded.
    /// </summary>
    /// <param name="record">The record of a auction to be handled.</param>
    /// <returns>The task object representing the asynchronous handling.</returns>
    Task HandleRecordAsync(BaseAuctionRecord record);

    /// <summary>
    /// Updates the auction record with the <paramref name="id"/>.
    /// </summary>
    /// <param name="id">Id of the ending auction record.</param>
    /// <param name="record">The current data of the auction.</param>
    /// <returns>The task object representing the asynchronous updating.</returns>
    Task UpdateEndingAuctionAsync(Guid id, BaseAuctionRecord record);
}
