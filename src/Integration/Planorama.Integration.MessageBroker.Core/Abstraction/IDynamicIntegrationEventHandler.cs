using System.Threading.Tasks;

namespace Planorama.Integration.MessageBroker.Core.Abstraction
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}
