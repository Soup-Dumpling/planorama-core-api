using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planorama.Integration.MessageBroker.Core.Abstraction;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Planorama.Integration.MessageBroker.RabbitMq
{
    public class RabbitMqEventBusHostedService : IHostedService
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<RabbitMqEventBusHostedService> _logger;

        public RabbitMqEventBusHostedService(IEventBus eventBus, ILogger<RabbitMqEventBusHostedService> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting RabbitMQ Event Bus Hosted Service");

            try
            {
                await _eventBus.InitializeAsync();
                _logger.LogInformation("RabbitMQ Event Bus successfully initialized and consuming");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while initializing RabbitMQ Event Bus");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping RabbitMQ Event Bus Hosted Service");

            if (_eventBus is IDisposable disposable)
            {
                try
                {
                    disposable.Dispose();
                    _logger.LogInformation("RabbitMQ Event Bus disposed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while disposing RabbitMQ Event Bus");
                }
            }

            _logger.LogInformation("RabbitMQ Event Bus Hosted Service stopped");
            return Task.CompletedTask;
        }
    }
}
