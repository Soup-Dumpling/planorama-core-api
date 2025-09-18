using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planorama.Notification.Core.UseCases.Notification.AddNotification;
using Planorama.Notification.Core.UseCases.Notification.GetNotifications;
using Planorama.Notification.Infrastructure.Repository.Notification;
using System.Reflection;

namespace Planorama.Notification.Infrastructure
{
    public static class Startup
    {
        public static void AddInfrastructureBindings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NotificationContext>(opt =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                opt.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                });
            });
            services.AddScoped<IAddNotificationRepository, AddNotificationRepository>();
            services.AddScoped<IGetNotificationsRepository, GetNotificationsRepository>();
        }
    }
}
