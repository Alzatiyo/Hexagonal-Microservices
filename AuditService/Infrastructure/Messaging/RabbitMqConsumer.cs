using Application.Dtos;
using Application.Ports.In;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Adapters.Messaging
{
    /// <summary>
    ///     Inbound adapter: consumes product events from RabbitMQ queue "product-events"
    ///     and creates audit records in MongoDB.
    /// </summary>
    public class RabbitMqConsumerAdapter : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMqConsumerAdapter> _logger;
        private readonly IConnection _connection;
        private IModel? _channel;
        private const string QueueName = "product-events";

        public RabbitMqConsumerAdapter(
            IConnection connection,
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMqConsumerAdapter> logger)
        {
            _connection = connection;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Misma declaración que hace RabbitMqPublisher en Stock_Hexagonal
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var productEvent = JsonSerializer.Deserialize<ProductEventMessage>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (productEvent is not null)
                        await HandleEventAsync(productEvent);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RabbitMQ message");
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer);

            // Mantener el BackgroundService vivo
            return Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleEventAsync(ProductEventMessage productEvent)
        {
            using var scope = _scopeFactory.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<IAuditUseCasePort>();

            var request = new CreateAuditRequest
            {
                Action = productEvent.Action,
                EntityName = $"Product/{productEvent.ProductName}",
                User = "Stock_Hexagonal",
                Details = $"Id: {productEvent.ProductId} | Stock: {productEvent.Stock} | Price: {productEvent.Price} | Status: {productEvent.Status}"
            };

            await useCase.CreateAsync(request);

            _logger.LogInformation("Audit created — Action: {Action} | Product: {Product}",
                productEvent.Action, productEvent.ProductName);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            base.Dispose();
        }

        // Espejo exacto de ProductEvent en Stock_Hexagonal
        private class ProductEventMessage
        {
            public Guid EventId { get; set; }
            public string Action { get; set; } = string.Empty;
            public Guid ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string ProductDescription { get; set; } = string.Empty;
            public int Stock { get; set; }
            public decimal Price { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime OccurredAt { get; set; }
        }
    }
}
