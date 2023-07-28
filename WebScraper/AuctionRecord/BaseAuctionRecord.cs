namespace WebScraper.AuctionRecord
{
    /// <summary>
    /// Base implementation of any auction <c>record</c> entity.
    /// </summary>
    public class BaseAuctionRecord
    {
        /// <summary>
        /// Gets the id of the record.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets the record's time of creation.
        /// </summary>
        public DateTime Created { get; init; }

        /// <summary>
        /// Gets the time when the auction was created.
        /// </summary>
        public DateTime StartOfAuction { get; init; }

        /// <summary>
        /// Gets the time when the auction will end.
        /// </summary>
        public DateTime EndOfAuction { get; init; }

        /// <summary>
        /// Gets the name of the auction.
        /// </summary>
        public string Name { get; init; } = string.Empty;

    }
}
