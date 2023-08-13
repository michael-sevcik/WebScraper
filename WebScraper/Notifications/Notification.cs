namespace WebScraper.Notifications;

/// <summary>
/// Represents a notification of an event.
/// </summary>
internal readonly struct Notification
{
    /// <summary>
    /// What kind of an event occurred.
    /// </summary>
    public readonly string Purpose;

    /// <summary>
    /// Short description of the event.
    /// </summary>
    public readonly string Title;

    /// <summary>
    /// The description of the event.
    /// </summary>
    public readonly string Message;
}
