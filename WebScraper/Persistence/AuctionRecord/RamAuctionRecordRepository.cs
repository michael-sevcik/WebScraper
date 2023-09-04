using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Persistence.AuctionRecord
{
    internal class RamAuctionRecordRepository : IAuctionRecordRepository
    {
        private readonly Dictionary<Guid, BaseAuctionRecord> baseAuctionRecords = new();

        public Task AddAsync(BaseAuctionRecord record)
        {
            this.baseAuctionRecords[record.Id] = record;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            this.baseAuctionRecords.Remove(id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<BaseAuctionRecord>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<BaseAuctionRecord>>(this.baseAuctionRecords.Values);
        }

        public Task<IEnumerable<BaseAuctionRecord>> GetAllEndingFromDateAsync(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Task<BaseAuctionRecord> GetAsync(Guid id)
        {
            return Task.FromResult(this.baseAuctionRecords[id]);
        }

        public Task<BaseAuctionRecord?> GetOrDefault(string uniqueIdentifier)
        {
            var record = this.baseAuctionRecords.Values.SingleOrDefault(e => e.UniqueIdentifier == uniqueIdentifier);
            return Task.FromResult(record);
        }

        public Task UpdateAsync(BaseAuctionRecord record)
        {
            this.baseAuctionRecords[record.Id] = record;
            return Task.CompletedTask;
        }
    }
}
