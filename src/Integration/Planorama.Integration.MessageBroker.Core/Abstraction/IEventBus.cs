using Planorama.Integration.MessageBroker.Core.Events;
using System.Threading.Tasks;

namespace Planorama.Integration.MessageBroker.Core.Abstraction
{
    public interface IEventBus
    {
        Task InitializeAsync();
        Task PublishAsync(ServiceBusEvent @event);

        Task SubscribeAsync<T, TH>()
            where T : ServiceBusEvent
            where TH : IIntegrationEventHandler<T>;

        Task SubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : ServiceBusEvent;
    }
}
