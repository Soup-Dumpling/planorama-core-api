using Alba;
using Microsoft.Extensions.DependencyInjection;
using Planorama.Notification.Infrastructure;
using System;
using System.Linq;

namespace Planorama.Notification.API.IntegrationTests.Extentions
{
    public static class AlbaHostExtentions
    {
        public static void WithEmptyDatabase(this IAlbaHost host, Action<NotificationContext> action)
        {
            var scopeFactory = host.Server.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<NotificationContext>();
            context.RemoveRange(context.Notifications.ToList());
            context.SaveChanges();
            action(context);
        }
    }
}
