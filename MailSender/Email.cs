using MimeKit;

namespace MailSender;

/// <summary>
/// Struct <c>Email</c> encapsulates its Subject and message.
/// </summary>
public readonly struct Email
{
    /// <summary>
    /// The recipient's email address.
    /// </summary>
    public readonly MailboxAddress Recepient;

    /// <summary>
    /// The Subject of the email.
    /// </summary>
    public readonly string Subject;

    /// <summary>
    /// The content of an email that is displayed when addressee opens it. 
    /// </summary>
    public readonly string Body;

    /// <summary>
    /// Creates an instance of <see cref="Email"/>.
    /// </summary>
    /// <param name="recipient">The recipient's email address.</param>
    /// <param name="subject">The Subject of the email.</param>
    /// <param name="body">The Body of the email.</param>
    public Email(MailboxAddress recipient, string subject, string body)
        => (this.Recepient, this.Subject, this.Body) = (recipient, subject, body);
}
