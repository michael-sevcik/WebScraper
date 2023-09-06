namespace WebScraper.Notifications;

/// <summary>
/// Represents an objects that sends notifications.
/// </summary>
public interface INotifier
{
    /// <summary>
    /// Asynchronously sends the <paramref name="notification"/>.
    /// </summary>
    /// <param name="notification">The description of the new event.</param>
    /// <returns>Task object representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Notification failed.</exception>
    Task NotifyAsync(Notification notification);
}
