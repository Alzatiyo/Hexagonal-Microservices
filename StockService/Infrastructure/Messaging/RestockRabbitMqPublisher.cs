using Aplication.Ports.Out;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Adapters.Messaging
{
    /// <summary>
    ///     Publica eventos de reabastecimiento en la cola "stock-restock-requests"
    ///     para que SuppliersMS los consuma y encuentre el proveedor adecuado.
    /// </summary>
    public class RestockRabbitMqPublisher : IRestockPublisherPort, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "stock-restock-requests";

        public RestockRabbitMqPublisher(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:Username"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public Task PublishRestockRequestAsync(RestockRequestEvent restockEvent)
        {
            var json = JsonSerializer.Serialize(restockEvent);
            var body = Encoding.UTF8.GetBytes(json);

            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: QueueName,
                basicProperties: props,
                body: body);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}