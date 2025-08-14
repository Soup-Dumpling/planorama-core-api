using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace Planorama.Integration.MessageBroker.RabbitMq
{
    public interface IRabbitMqPersistentConnection : IDisposable
    {
        bool IsConnected { get; }
        Task<bool> TryConnectAsync();
        Task<IChannel> CreateChannelAsync();
    }
}
