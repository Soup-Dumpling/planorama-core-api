using Alba;
using Microsoft.Extensions.DependencyInjection;
using Planorama.User.Infrastructure;
using System;
using System.Linq;

namespace Planorama.User.API.IntegrationTests.Extentions
{
    public static class AlbaHostExtentions
    {
        public static void WithEmptyDatabase(this IAlbaHost host, Action<UserDBContext> action)
        {
            var scopeFactory = host.Server.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<UserDBContext>();
                context.RemoveRange(context.Users.ToList());
                context.RemoveRange(context.UserCredentials.ToList());
                context.RemoveRange(context.Roles.ToList());
                context.RemoveRange(context.UserRoles.ToList());
                context.RemoveRange(context.UserPrivacySettings.ToList());
                context.SaveChanges();
                action(context);
            }
        }
    }
}
