using Aplication.Ports.In;
using Microsoft.Extensions.Configuration;
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
    ///     Servicio en background que consume la cola "stock-restock-requests"
    ///     publicada por Stock_Hexagonal cuando un producto cae por debajo
    ///     del stock mínimo.
    ///     Al recibir el mensaje, consulta el UseCase para obtener el primer
    ///     proveedor disponible y lo registra en el log.
    /// </summary>
    public class RestockRequestConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RestockRequestConsumer> _logger;
        private IConnection? _connection;
        private IModel? _channel;
        private const string QueueName = "stock-restock-requests";

        public RestockRequestConsumer(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<RestockRequestConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:Username"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declaramos la misma cola que publica Stock_Hexagonal (idempotente)
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _logger.LogInformation("RestockRequestConsumer iniciado. Escuchando cola: {Queue}", QueueName);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Capturamos en variable local no-nullable para evitar warnings del compilador
            // dentro del lambda asíncrono — _channel siempre está inicializado en StartAsync.
            var channel = _channel ?? throw new InvalidOperationException("Channel not initialized.");

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<RestockRequestMessage>(json);

                    if (message is null)
                    {
                        _logger.LogWarning("Mensaje de reabastecimiento nulo o inválido recibido.");
                        channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                        return;
                    }

                    _logger.LogInformation(
                        "Solicitud de reabastecimiento recibida. ProductId={ProductId} | Stock={Stock} | Mínimo={Min}",
                        message.ProductId, message.CurrentStock, message.StockMinimum);

                    // ISupplierUseCasePort es Scoped → creamos un scope por mensaje
                    using var scope = _scopeFactory.CreateScope();
                    var supplierUseCase = scope.ServiceProvider.GetRequiredService<ISupplierUseCasePort>();

                    var suppliers = await supplierUseCase.GetAvailableByProductAsync(message.ProductId);
                    var first = suppliers.FirstOrDefault();

                    if (first is null)
                    {
                        _logger.LogWarning(
                            "No se encontraron proveedores disponibles para ProductId={ProductId}.",
                            message.ProductId);
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Proveedor seleccionado para reabastecimiento: {Supplier}",
                            JsonSerializer.Serialize(first));
                    }

                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando mensaje de reabastecimiento.");
                    channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }

        // DTO interno para deserializar el mensaje de Stock_Hexagonal
        private sealed record RestockRequestMessage(
            Guid EventId,
            Guid ProductId,
            string ProductName,
            int CurrentStock,
            int StockMinimum,
            DateTime OccurredAt);
    }
}