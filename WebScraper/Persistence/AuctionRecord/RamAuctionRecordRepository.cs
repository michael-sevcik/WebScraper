using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Persistence.AuctionRecord
{
    /// <summary>
    /// Dictionary based auction repository.
    /// </summary>
    internal class RamAuctionRecordRepository : IAuctionRecordRepository
    {
        private readonly Dictionary<Guid, BaseAuctionRecord> baseAuctionRecords = new();

        /// <inheritdoc/>
        public Task AddAsync(BaseAuctionRecord record)
        {
            this.baseAuctionRecords[record.Id] = record;
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task DeleteAsync(Guid id)
        {
            this.baseAuctionRecords.Remove(id);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<BaseAuctionRecord>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<BaseAuctionRecord>>(this.baseAuctionRecords.Values);
        }

        public Task<HashSet<string>> GetAllUniqueIdentifiersAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<BaseAuctionRecord>> GetAllEndingToDateAsync(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<BaseAuctionRecord> GetAsync(Guid id)
        {
            return Task.FromResult(this.baseAuctionRecords[id]);
        }

        /// <inheritdoc/>
        public Task<BaseAuctionRecord?> GetOrDefault(string uniqueIdentifier)
        {
            var record = this.baseAuctionRecords.Values.SingleOrDefault(e => e.UniqueIdentifier == uniqueIdentifier);
            return Task.FromResult(record);
        }

        /// <inheritdoc/>
        public Task UpdateAsync(BaseAuctionRecord record)
        {
            this.baseAuctionRecords[record.Id] = record;
            return Task.CompletedTask;
        }
    }
}
