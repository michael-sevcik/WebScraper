using MailSender;
using MimeKit;

namespace Application.Configuration;

[Serializable]
internal class EmailNotificationConfiguration
{
    public EmailNotificationConfiguration(SmtpConfiguration SmtpConfiguration, string Recipient)
    {
        this.SmtpConfiguration=SmtpConfiguration;
        this.Recipient=Recipient;
    }

    public SmtpConfiguration SmtpConfiguration { get; set; }
    public string Recipient { get; set; }
}
