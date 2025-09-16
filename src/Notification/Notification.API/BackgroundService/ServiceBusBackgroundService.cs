using Planorama.Integration.MessageBroker.Core.Abstraction;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.Notification.API.BackgroundService
{
    public class ServiceBusBackgroundService : IHostedService
    {
        private readonly IEventBus eventBus;

        public ServiceBusBackgroundService(IEventBus eventBus) 
        {
            this.eventBus = eventBus;
        }

        public Task StartAsync(CancellationToken cancellationToken) 
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) 
        {
            return Task.CompletedTask;
        }
    }
}
