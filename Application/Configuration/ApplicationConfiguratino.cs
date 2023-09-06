using MailSender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Configuration;

namespace Application.Configuration
{
    internal class ApplicationConfiguration
    {
        public ApplicationConfiguration(EmailNotificationConfiguration smtpConfiguration, WebScraperConfiguration emailNotificationConfiguration)
            => (this.EmailNotificationConfiguration, this.WebScraperConfiguration) = (smtpConfiguration, emailNotificationConfiguration);

        public EmailNotificationConfiguration EmailNotificationConfiguration { get; }
        public WebScraperConfiguration WebScraperConfiguration { get; }
    }
}
