using System;
using System.Text.Json;

namespace Planorama.User.Core.Models
{
    public class IntegrationEvent
    {
        public Guid Id { get; protected set; }
        public string EventType { get; protected set; }
        public string AggregationId { get; protected set; }
        public string Data { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public string Username { get; protected set; }
    }

    public class IntegrationEvent<T> : IntegrationEvent
    {
        public IntegrationEvent(string aggregationId, T data, string username)
        {
            Id = Guid.NewGuid();
            EventType = typeof(T).Name;
            AggregationId = aggregationId;
            Data = JsonSerializer.Serialize(data);
            Timestamp = DateTime.UtcNow;
            Username = username;
        }
    }
}
