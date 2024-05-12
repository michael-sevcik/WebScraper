using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace MailSender;

/// <summary>
/// Implementation of <see cref="IMailSender"/> that uses <see cref="SmtpClient"/> to send emails.
/// </summary>
public class MailKitSender : IMailSender
{
    private readonly SmtpConfiguration smtpConfiguration;

    /// <summary>
    /// Creates a new instance of <see cref="MailKitSender"/>.
    /// </summary>
    /// <param name="smtpConfiguration">The SMTP connection configuration.</param>
    public MailKitSender(SmtpConfiguration smtpConfiguration)
        => this.smtpConfiguration = smtpConfiguration;

    /// <inheritdoc/>
    public async Task SendEmailAsync(Email email, CancellationToken ct = default)
    {
        // Create the message
        var message = new MimeMessage();
        message.To.Add(email.Recepient);

        // Sender
        MailboxAddress sender = MailboxAddress.Parse(this.smtpConfiguration.Sender);
        message.From.Add(sender);
        message.Sender = sender;

        message.Subject = email.Subject;

        // Create body
        BodyBuilder bodyBuilder = new()
        {
            TextBody = email.Body
        };

        message.Body = bodyBuilder.ToMessageBody();

        // Send the message
        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(smtpConfiguration.Host, this.smtpConfiguration.Port, SecureSocketOptions.Auto, ct);

        await smtp.AuthenticateAsync(this.smtpConfiguration.UserName, this.smtpConfiguration.Password, ct);
        await smtp.SendAsync(message, ct);
        await smtp.DisconnectAsync(true, ct);
    }
}
