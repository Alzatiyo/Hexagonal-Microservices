using Aplication.Ports.Out;
using Domain.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Adapters.Messaging
{
    /// <summary>
    ///     Adaptador de infraestructura que implementa IEventPublisherPort usando RabbitMQ.
    ///     Publica los eventos de producto en la cola "product-events".
    /// </summary>
    public class RabbitMqPublisher : IEventPublisherPort, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string QueueName = "product-events";

        public RabbitMqPublisher(IConfiguration configuration)
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

            // Cola duradera: los mensajes sobreviven reinicios del broker
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public Task PublishAsync(ProductEvent productEvent)
        {
            var json = JsonSerializer.Serialize(productEvent);
            var body = Encoding.UTF8.GetBytes(json);

            // Persistent: el mensaje sobrevive reinicios de RabbitMQ
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