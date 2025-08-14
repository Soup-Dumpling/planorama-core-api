using Planorama.Integration.MessageBroker.Core.Events;
using System.Threading.Tasks;

namespace Planorama.Integration.MessageBroker.Core.Abstraction
{
    public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : ServiceBusEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
