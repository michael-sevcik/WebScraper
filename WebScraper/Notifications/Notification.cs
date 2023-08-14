namespace WebScraper.Notifications;

/// <summary>
/// Represents a notification of an event.
/// </summary>
internal readonly struct Notification
{
    /// <summary>
    /// What kind of an event occurred.
    /// </summary>
    public readonly string Reason;

    /// <summary>
    /// Short description of the event.
    /// </summary>
    public readonly string Title;

    /// <summary>
    /// The description of the event.
    /// </summary>
    public readonly string Message;

    /// <summary>
    /// Initializes a new instance of the <see cref="Notification"/> struct.
    /// </summary>
    /// <param name="reason">What kind of an event occurred.</param>
    /// <param name="tilte">Short description of the event.</param>
    /// <param name="message">Full description of the event.</param>
    public Notification(string reason, string tilte, string message)
        => (this.Reason, this.Title, this.Message) = (reason, tilte, message);
}
