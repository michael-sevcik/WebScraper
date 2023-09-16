using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebScraper.Notifications;

namespace WebScraperTests.Mocks;

internal class MockNotifier : INotifier
{
    public int NotificationCount { get; private set; }
    public Task NotifyAsync(Notification notification)
    {
        ++this.NotificationCount;
        return Task.CompletedTask;
    }
}
