using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailSender
{
    /// <summary>
    /// Utility class to send emails.
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// Asynchronously sends the <paramref name="email"/>.
        /// </summary>
        /// <param name="email">The email to be send.</param>
        /// <returns>Email sending task.</returns>
        Task SendEmailAsync(Email email);
    }
}
