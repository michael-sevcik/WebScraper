namespace WebScraper.Utils
{
    /// <summary>
    /// Represents a class that provides time.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the DateTime object representing the current application time.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the DateTime object representing the current application time in UTC.
        /// </summary>
        DateTime UtcNow { get; }
    }
}
