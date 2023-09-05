using Microsoft.Extensions.Logging;

namespace WebScraper.Notifications;

/// <summary>
/// <see cref="INotifier"/> implementation that logs the notifications.
/// </summary>
public sealed class LogNotifier : INotifier
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogNotifier"/> class.
    /// </summary>
    /// <param name="logger">The logger instance that is supposed to be used for logging notifications.</param>
    public LogNotifier(ILogger<LogNotifier> logger)
        => this.logger = logger;

    /// <inheritdoc/>
    public Task NotifyAsync(Notification notification)
    {
        var message = """
            {reason}: {Title}
            --------------------

            {body}
            """;

        this.logger.LogInformation(message, notification.Reason, notification.Title, notification.Message);
        return Task.CompletedTask;
    }
}
