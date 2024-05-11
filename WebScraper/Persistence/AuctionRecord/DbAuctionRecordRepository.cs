using Microsoft.EntityFrameworkCore;

namespace WebScraper.Persistence.AuctionRecord;

/// <summary>
/// Implementation of <see cref="IAuctionRecordRepository"/> that enables managing and storing
/// of <see cref="BaseAuctionRecord"/> entities using an instance of <see cref="ScraperDbContext"/>.
/// </summary>
internal class DbAuctionRecordRepository : IAuctionRecordRepository
{
    private readonly DbSet<BaseAuctionRecord> records;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbAuctionRecordRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The db context that should be used to store the records.</param>
    public DbAuctionRecordRepository(ScraperDbContext dbContext)
    {
        this.records = dbContext.AuctionRecords;
    }

    /// <inheritdoc/>
    public async Task AddAsync(BaseAuctionRecord record)
    {
        await this.records.AddAsync(record);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        var record = await this.records.FindAsync(id) ??
            throw new InvalidOperationException($"The record with id {id} was not found.");

        this.records.Remove(record);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<BaseAuctionRecord>> GetAllAsync()
        => Task.FromResult(this.records.AsNoTracking().AsEnumerable());

    /// <inheritdoc/>
    public Task<HashSet<string>> GetAllUniqueIdentifiersAsync()
        => Task.Run(() => this.records.AsNoTracking().Select(e => e.UniqueIdentifier).ToHashSet());

    /// <inheritdoc/>
    public Task<IEnumerable<BaseAuctionRecord>> GetAllEndingToDateAsync(DateTime dateTime)
    {
        var filteredRecords = this.records.Where(e => e.EndOfAuction < dateTime).AsEnumerable();
        return Task.FromResult(filteredRecords);
    }

    /// <inheritdoc/>
    public async Task<BaseAuctionRecord> GetAsync(Guid id)
    {
        var record = await this.records.FindAsync(id) ??
            throw new InvalidOperationException($"No record with id {id} was found.");

        this.records.Entry(record).State = EntityState.Detached;
        return record;
    }

    /// <inheritdoc/>
    public async Task<BaseAuctionRecord?> GetOrDefault(string uniqueIdentifier)
    => await this.records.AsNoTracking().SingleOrDefaultAsync(e => e.UniqueIdentifier == uniqueIdentifier);

    /// <inheritdoc/>
    public Task UpdateAsync(BaseAuctionRecord record)
    {
        this.records.Entry(record).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
