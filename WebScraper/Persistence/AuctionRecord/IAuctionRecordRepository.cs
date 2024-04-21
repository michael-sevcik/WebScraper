namespace WebScraper.Persistence.AuctionRecord
{
    /// <summary>
    /// Represents an auction record repository that enables storing and managing <see cref="BaseAuctionRecord"/> entities.
    /// </summary>
    public interface IAuctionRecordRepository
    {
        /// <summary>
        /// Asynchronously tries to get the <see cref="BaseAuctionRecord"/> entity with the specified <paramref name="uniqueIdentifier"/>.
        /// </summary>
        /// <param name="uniqueIdentifier">
        /// <see cref="BaseAuctionRecord.UniqueIdentifier"/> of the desired <see cref="BaseAuctionRecord"/> entity.
        /// </param>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object
        /// returns the record with the <paramref name="uniqueIdentifier"/>, if it exists, otherwise null.
        /// </returns>
        Task<BaseAuctionRecord?> GetOrDefault(string uniqueIdentifier);

        /// <summary>
        /// Gets asynchronously the <see cref="BaseAuctionRecord"/> entity with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id"><see cref="BaseAuctionRecord.Id"/> of the desired <see cref="BaseAuctionRecord"/> entity.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object returns
        /// the <see cref="BaseAuctionRecord"/> entity with the <paramref name="id"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException"> Is thrown in case that an entity with the specified <paramref name="id"/> is not found.</exception>
        Task<BaseAuctionRecord> GetAsync(Guid id);

        /// <summary>
        /// Gets asynchronously all the <see cref="BaseAuctionRecord"/> entities.
        /// </summary>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object
        /// returns <see cref="IEnumerable{T}"/> containing all records.
        /// </returns>
        Task<IEnumerable<BaseAuctionRecord>> GetAllAsync();

        /// <summary>
        /// Gets asynchronously all the unique identifiers of the <see cref="BaseAuctionRecord"/> entities.
        /// </summary>
        /// <returns>Set of unique identifiers.</returns>
        Task<HashSet<string>> GetAllUniqueIdentifiersAsync();

        /// <summary>
        /// Gets asynchronously all the <see cref="BaseAuctionRecord"/> entities.
        /// </summary>
        /// <param name="dateTime">The date used for filtering.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object
        /// returns <see cref="IEnumerable{T}"/> containing all records.
        /// </returns>
        Task<IEnumerable<BaseAuctionRecord>> GetAllEndingToDateAsync(DateTime dateTime);

        /// <summary>
        /// Asynchronously stores the new <paramref name="record"/>.
        /// </summary>
        /// <param name="record">The record to add.</param>
        /// <returns>The task object representing the asynchronous storing.</returns>
        Task AddAsync(BaseAuctionRecord record);

        /// <summary>
        /// Deletes the specified <see cref="BaseAuctionRecord"/> entity as an asynchronous operation using a <see cref="Task"/> object.
        /// </summary>
        /// <param name="id"><see cref="BaseAuctionRecord.Id"/> of the desired <see cref="BaseAuctionRecord"/> entity.</param>
        /// <returns>The task object representing the asynchronous deletion.</returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Updates the stored data of the <see cref="BaseAuctionRecord"/> entity with matching <see cref="BaseAuctionRecord.Id"/> as an asynchronous operation using a <see cref="Task"/> object.
        /// </summary>
        /// <param name="record">The entity to be updated.</param>
        /// <returns>The task object representing the asynchronous updating.</returns>
        Task UpdateAsync(BaseAuctionRecord record);
    }
}
