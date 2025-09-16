using Microsoft.EntityFrameworkCore;
using System;

namespace Planorama.Notification.Infrastructure.UnitTests.Helpers
{
    public static class InMemoryContextHelper
    {
        public static NotificationContext GetContext()
        {
            var builder = new DbContextOptionsBuilder<NotificationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString());
            return new NotificationContext(builder.Options);
        }
    }
}
