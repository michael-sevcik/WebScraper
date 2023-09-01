using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper.Persistence.AuctionRecord;

public class AuctionRecordRepository : IAuctionRecordRepository
{


    public Task AddAsync(BaseAuctionRecord record)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BaseAuctionRecord>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BaseAuctionRecord>> GetAllFromDateAsync(DateTime dateTime)
    {
        throw new NotImplementedException();
    }

    public Task<BaseAuctionRecord> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryGetAsync(string uniqueIdentifier, out BaseAuctionRecord record)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(BaseAuctionRecord record)
    {
        throw new NotImplementedException();
    }
}
