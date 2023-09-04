﻿using MailSender;

namespace WebScraper.Notifications;

/// <summary>
/// An implementation of <see cref="INotifier"/> that sends the notifications using an instance of <see cref="IMailSender"/>.
/// </summary>
internal class EmailNotifier : INotifier
{
    private readonly IMailSender mailSender;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotifier"/> class.
    /// </summary>
    /// <param name="mailSender">The mail sender that is used for sending the email notifications.</param>
    public EmailNotifier(IMailSender mailSender)
        => this.mailSender = mailSender;

    /// <inheritdoc/>
    public async Task NotifyAsync(Notification notification)
    {
        // Format the email
        var subject = $"{notification.Reason}: {notification.Title}";
        var body = notification.Message;

        // Create and send the email
        await this.mailSender.SendEmailAsync(new(subject, body));
    }
}