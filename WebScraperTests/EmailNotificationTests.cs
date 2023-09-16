using Application.Parsing;
using Downloader;
using MailSender;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using netDumbster.smtp;
using Quartz;
using System.Data;
using System.Text.Json;
using Testcontainers.MsSql;
using WebScraper;
using WebScraper.Configuration;
using WebScraper.Notifications;
using WebScraper.Persistence;
using WebScraper.Scraping;
using WebScraper.Utils;
using WebScraperTests.Mocks;

namespace WebScraperTests;

public class EmailNotificationTests
{
    [Test]
    public async Task TestEmailNotifier()
    {
        var port = 9009;
        var server = SimpleSmtpServer.Start(port);

        var config = server.Configuration;

        MailKitSender mailKitSender = new(new("user@example.com", "strongPassword", "localhost", config.Port, "user@example.com"));
        EmailNotifier emailNotifier = new(mailKitSender, "user2@example.com");

        await emailNotifier.NotifyAsync(new("Test1", "Test2", "Test3"));



        Assert.Multiple(() =>
        {
            Assert.That(server.ReceivedEmailCount, Is.EqualTo(1));
            var email = server.ReceivedEmail[0];
            Assert.That(email.Subject, Is.EqualTo("Test1: Test2"));
        });
    }
}