using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.Integration.MessageBroker.Core.Events;
using System.Threading.Tasks;

namespace Planorama.Notification.API.IntegrationTests.Fakes
{
    public class FakeEventBus : IEventBus
    {
        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }
        public async Task PublishAsync(ServiceBusEvent @event)
        {
            await Task.CompletedTask;
        }

        public async Task SubscribeAsync<T, TH>()
            where T : ServiceBusEvent
            where TH : IIntegrationEventHandler<T>
        {
            await Task.CompletedTask;
        }

        public async Task SubscribeDynamicAsync<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            await Task.CompletedTask;
        }

        public void Unsubscribe<T, TH>()
            where T : ServiceBusEvent
            where TH : IIntegrationEventHandler<T>
        {
            return;
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            return;
        }
    }
}
