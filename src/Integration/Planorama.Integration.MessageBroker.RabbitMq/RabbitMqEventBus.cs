using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Planorama.Integration.MessageBroker.Core;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using Planorama.Integration.MessageBroker.Core.Events;
using Planorama.Integration.MessageBroker.Core.Extensions;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Planorama.Integration.MessageBroker.RabbitMq
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private const string BROKER_NAME = "planorama_event_bus";

        private readonly IRabbitMqPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly int _retryCount;

        private IChannel _consumerChannel;
        private string _queueName;

        public RabbitMqEventBus(IRabbitMqPersistentConnection persistentConnection, ILogger<RabbitMqEventBus> logger,
            IEventBusSubscriptionsManager subsManager, IServiceScopeFactory serviceScopeFactory, int retryCount = 5, string queueName = null)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();
            this.serviceScopeFactory = serviceScopeFactory;
            _retryCount = retryCount;
            _queueName = queueName;

            _subsManager.OnEventRemoved += async (sender, eventName) => await RemoveSubscriptionAsync(eventName);
        }

        public async Task InitializeAsync()
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }
            _consumerChannel = await CreateConsumerChannelAsync();
            await StartBasicConsumeAsync();
        }

        private async Task RemoveSubscriptionAsync(string eventName)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }

            using (var channel = await _persistentConnection.CreateChannelAsync())
            {
                await channel.QueueUnbindAsync(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);

                if (_subsManager.IsEmpty)
                {
                    _queueName = string.Empty;
                    await _consumerChannel.CloseAsync();
                }
            }
        }

        public async Task PublishAsync(ServiceBusEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = @event.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using (var channel = await _persistentConnection.CreateChannelAsync())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

                await channel.ExchangeDeclareAsync(exchange: BROKER_NAME, type: "direct");

                var body = JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                });

                await policy.ExecuteAsync(async () =>
                {
                    var properties = new BasicProperties
                    {
                        DeliveryMode = DeliveryModes.Persistent,
                    };

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    await channel.BasicPublishAsync(
                        exchange: BROKER_NAME,
                        routingKey: eventName,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public async Task SubscribeDynamicAsync<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation("Subscribing to dynamic event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

            await DoInternalSubscriptionAsync(eventName);
            _subsManager.AddDynamicSubscription<TH>(eventName);
            await StartBasicConsumeAsync();
        }

        public async Task SubscribeAsync<T, TH>()
            where T : ServiceBusEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            await DoInternalSubscriptionAsync(eventName);

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).GetGenericTypeName());

            _subsManager.AddSubscription<T, TH>();
            await StartBasicConsumeAsync();
        }

        private async Task DoInternalSubscriptionAsync(string eventName)
        {
            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_persistentConnection.IsConnected)
                {
                    await _persistentConnection.TryConnectAsync();
                }
                await _consumerChannel.QueueBindAsync(queue: _queueName,
                    exchange: BROKER_NAME,
                    routingKey: eventName);
            }
        }

        public void Unsubscribe<T, TH>()
            where T : ServiceBusEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();
            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
            _subsManager.Clear();
        }

        private async Task StartBasicConsumeAsync()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

                consumer.ReceivedAsync += Consumer_ReceivedAsync;

                await _consumerChannel.BasicConsumeAsync(
                    queue: _queueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                _logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_ReceivedAsync(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                await ProcessEventAsync(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // Even on exception we take the message off the queue.
            // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
            // For more information see: https://www.rabbitmq.com/dlx.html
            await _consumerChannel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        }

        private async Task<IChannel> CreateConsumerChannelAsync()
        {
            if (!_persistentConnection.IsConnected)
            {
                await _persistentConnection.TryConnectAsync();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = await _persistentConnection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: BROKER_NAME,
                type: "direct");

            await channel.QueueDeclareAsync(queue: _queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.CallbackExceptionAsync += async (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = await CreateConsumerChannelAsync();
                await StartBasicConsumeAsync();
            };
            return channel;
        }

        private async Task ProcessEventAsync(string eventName, string message)
        {
            _logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler = scope.ServiceProvider.GetService(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            if (handler == null) continue;
                            using dynamic eventData = JsonDocument.Parse(message);
                            await Task.Yield();
                            await handler.Handle(eventData);
                        }
                        else
                        {
                            var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                            if (handler == null) continue;
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonSerializer.Deserialize(message, eventType, new JsonSerializerOptions()
                            {
                                PropertyNameCaseInsensitive = true,
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                Converters = { new JsonStringEnumConverter() }
                            });
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }
    }
}
