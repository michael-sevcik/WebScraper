using MailSender;
using MimeKit;

namespace WebScraper.Notifications;

/// <summary>
/// An implementation of <see cref="INotifier"/> that sends the notifications using an instance of <see cref="IMailSender"/>.
/// </summary>
public class EmailNotifier : INotifier
{
    private readonly IMailSender mailSender;
    private readonly MailboxAddress recipient;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotifier"/> class.
    /// </summary>
    /// <param name="mailSender">The mail sender that is used for sending the email notifications.</param>
    /// <param name="recipient">Mail address to which the notifications should be sent.</param>
    public EmailNotifier(IMailSender mailSender, string recipient)
        => (this.mailSender, this.recipient )= (mailSender, MailboxAddress.Parse(recipient));

    /// <inheritdoc/>
    public async Task NotifyAsync(Notification notification)
    {
        // Format the email
        var subject = $"{notification.Reason}: {notification.Title}";
        var body = notification.Message;

        // Create and send the email
        await this.mailSender.SendEmailAsync(new(recipient, subject, body));
    }
}
