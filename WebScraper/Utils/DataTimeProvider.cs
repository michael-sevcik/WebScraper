namespace WebScraper.Utils
{
    /// <summary>
    /// Implementation of <see cref="IDateTimeProvider"/> that uses <see cref="DateTime.Now"/>.
    /// </summary>
    internal class DateTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc/>
        public DateTime Now => DateTime.Now;
    }
}
