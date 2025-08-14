using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.Integration.MessageBroker.Core.Events;
using System;
using System.Collections.Generic;

namespace Planorama.Integration.MessageBroker.Core
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void AddSubscription<T, TH>()
            where T : ServiceBusEvent
            where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
                where TH : IIntegrationEventHandler<T>
                where T : ServiceBusEvent;

        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionsForEvent<T>() where T : ServiceBusEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : ServiceBusEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
