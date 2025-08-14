using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planorama.Integration.MessageBroker.Core;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using RabbitMQ.Client;

namespace Planorama.Integration.MessageBroker.RabbitMq
{
    public static class Startup
    {
        public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRabbitMqPersistentConnection, RabbitMqPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RabbitMqPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"]
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new RabbitMqPersistentConnection(factory, logger, retryCount);
            });
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>(sp =>
            {
                var subscriptionClientName = configuration["SubscriptionClientName"];
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
                var iLifetimeScope = sp.GetRequiredService<IServiceScopeFactory>();
                var logger = sp.GetRequiredService<ILogger<RabbitMqEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new RabbitMqEventBus(rabbitMQPersistentConnection, logger, eventBusSubcriptionsManager, iLifetimeScope, retryCount, subscriptionClientName);
            });
            services.AddHostedService<RabbitMqEventBusHostedService>();
        }
    }
}
