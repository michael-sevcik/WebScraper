using WebScraper.Persistence.AuctionRecord;

namespace WebScraper.Persistence.UnitOfWork;

/// <summary>
/// Represents a unit of work, the work will be saved after calling <see cref="Complete"/> or <see cref="CompleteAsync"/>.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets an auction record manager.
    /// </summary>
    IAuctionRecordManager AuctionRecordManager { get; }

    /// <summary>
    /// Gets an auction record repository.
    /// </summary>
    IAuctionRecordRepository AuctionRecordRepository { get; }

    /// <summary>
    /// Saves the changes.
    /// </summary>
    void Complete();

    /// <summary>
    /// Saves the changes as an asynchronous operation.
    /// </summary>
    /// <returns>A task object representing the asynchronous operation.</returns>
    Task CompleteAsync();
}