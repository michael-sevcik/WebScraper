namespace WebScraper
{
    /// <summary>
    /// Base implementation of any <c>record</c> entity.
    /// </summary>
    public class BaseRecord
    {
        /// <summary>
        /// Gets or sets the id of the record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the record's time of creation.
        /// </summary>
        public DateTime Created { get; set; }
    }
}
