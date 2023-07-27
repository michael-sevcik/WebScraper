namespace MailSender;

/// <summary>
/// Struct <c>Email</c> encapsulates its subject and message.
/// </summary>
public readonly struct Email
{
    /// <summary>
    /// The subject of the email.
    /// </summary>
    public readonly string subject;

    /// <summary>
    /// The content of an email that is displayed when addressee opens it. 
    /// </summary>
    public readonly string body;
}
