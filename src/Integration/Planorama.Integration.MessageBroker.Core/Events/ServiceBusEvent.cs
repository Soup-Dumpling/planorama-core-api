using System;

namespace Planorama.Integration.MessageBroker.Core.Events
{
    public abstract record ServiceBusEvent(Guid Id);
}
