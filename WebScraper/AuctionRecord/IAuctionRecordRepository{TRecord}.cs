using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.AuctionRecord
{
    /// <summary>
    /// Represents generic auction record repository that enables storing and managing of <typeparamref name="TRecord"/> entities.
    /// </summary>
    /// <typeparam name="TRecord">Record type derived from <see cref="BaseAuctionRecord"/>.</typeparam>
    public interface IAuctionRecordRepository<TRecord>
        where TRecord : BaseAuctionRecord, new()
    {
        /// <summary>
        /// Gets asynchronously the <see cref="TRecord"/> entity with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id"><see cref="BaseAuctionRecord.Id"/> of the desired <see cref="TRecord"/> entity.</param>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object returns
        /// the <see cref="TRecord"/> entity with the <paramref name="id"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException"> Is thrown in case that an entity with the specified <paramref name="id"/> is not found.</exception>
        Task<TRecord> GetAsync(Guid id);

        /// <summary>
        /// Gets asynchronously all the <see cref="TRecord"/> entities.
        /// </summary>
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object
        /// returns <see cref="IEnumerable{T}"/> containing all records.
        /// </returns>
        Task<IEnumerable<TRecord>> GetAllAsync();

        /// <summary>
        /// Gets asynchronously all the <see cref="TRecord"/> entities.
        /// </summary>
        /// <param name="dateTime">The date used for filtering.</param> // TODO:
        /// <returns>
        /// The task object representing the asynchronous operation. The Result property on the task object
        /// returns <see cref="IEnumerable{T}"/> containing all records.
        /// </returns>
        Task<IEnumerable<TRecord>> GetAllFromDateAsync(DateTime dateTime);

        /// <summary>
        /// Asynchronously stores the new <paramref name="record"/>. // TODO: decide what should happen in case that the entity is already stored.
        /// </summary>
        /// <param name="record">The record to add.</param>
        /// <returns>
        /// The task object representing the asynchronous storing.</returns>
        Task AddAsync(TRecord record);

        /// <summary>
        /// Deletes the specified <see cref="TRecord"/> entity as an asynchronous operation using a <see cref="Task"/> object.
        /// </summary>
        /// <param name="id"><see cref="BaseAuctionRecord.Id"/> of the desired <see cref="TRecord"/> entity.</param>
        /// <returns>The task object representing the asynchronous deletion.</returns>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<TRecord>> GetRecordsAsync();

        /// <summary>
        /// Updates the stored data of the <see cref="TRecord"/> entity with matching <see cref="TRecord.Id"/> as an asynchronous operation using a <see cref="Task"/> object.
        /// </summary>
        /// <param name="record">The entity to be updated.</param>
        /// <returns>The task object representing the asynchronous updating.</returns>
        Task UpdateAsync(TRecord record);
    }
}
