using WebScraper.Persistence.AuctionRecord;

namespace WebScraper.Persistence.UnitOfWork
{
    /// <summary>
    /// An implementation of <see cref="IUnitOfWork"/>.
    /// </summary>
    internal sealed class ScraperUnitOfWork : IUnitOfWork
    {
        private readonly ScraperDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScraperUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        /// <param name="auctionRecordRepository">The auction repository.</param>
        /// <param name="auctionRecordManager">The auction record manager.</param>
        public ScraperUnitOfWork(
            ScraperDbContext context,
            IAuctionRecordRepository auctionRecordRepository,
            IAuctionRecordManager auctionRecordManager)
        {
            this.context = context;
            this.AuctionRecordRepository = auctionRecordRepository;
            this.AuctionRecordManager = auctionRecordManager;
        }

        /// <inheritdoc/>
        public IAuctionRecordRepository AuctionRecordRepository { get; }

        /// <inheritdoc/>
        public IAuctionRecordManager AuctionRecordManager { get; }

        /// <inheritdoc/>
        public void Complete()
        {
            this.context.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task CompleteAsync()
        {
            await this.context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Dispose method for derived entities.
        /// </summary>
        /// <param name="disposing">Are we disposing the derived object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.context.Dispose();
            }
        }
    }
}
