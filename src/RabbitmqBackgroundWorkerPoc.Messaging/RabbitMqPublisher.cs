using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly ConnectionFactory _factory;
        public RabbitMqPublisher(IConfiguration config)
        {
            var uri = config.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ connection string missing");
            _factory = new ConnectionFactory
            {
                Uri = new Uri(uri)
            };
        }
        public async Task PublishAsync(string queueName, QueueMessage message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
                                               
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, mandatory: false, body: body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
