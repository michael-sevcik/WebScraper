using WebScraper.Utils;

namespace WebScraperTests.Mocks;

internal class MockDateTimeProvider : IDateTimeProvider
{
    private readonly TimeSpan timeDiffrence;

    /// <summary>
    /// Initializes a new instance of <see cref="MockDateTimeProvider"/>.
    /// </summary>
    /// <param name="dateTime">The date from which this date time provider should start simulating time.</param>
    public MockDateTimeProvider(DateTime dateTime)
    {
        timeDiffrence = DateTime.Now - dateTime;
    }

    /// <inheritdoc/>
    public DateTime Now => DateTime.Now - timeDiffrence;

    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow - timeDiffrence;
}
