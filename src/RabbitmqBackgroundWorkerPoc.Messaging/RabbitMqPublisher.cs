using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly MessagingSettings _settings;
        private readonly ConnectionFactory _factory;
        public RabbitMqPublisher(IConfiguration config, IOptions<MessagingSettings> options)
        {
            _settings = options.Value;

            _factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.RabbitMqUri)
            };
        }
        public async Task PublishAsync(QueueMessage message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
                                               
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await channel.BasicPublishAsync(
                exchange: string.Empty, 
                routingKey: _settings.QueueName, 
                mandatory: false, 
                body: body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
