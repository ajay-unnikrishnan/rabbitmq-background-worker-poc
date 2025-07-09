using System.Text;
using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqClient : IMessageQueueClient
    {
        private readonly ConnectionFactory _factory;
        public RabbitMqClient(IConfiguration config)
        {
            var uri = config.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ connection string missing");
            _factory = new ConnectionFactory
            {
                Uri = new Uri(uri)
            };
        }
        public async Task PublishAsync(string queueName, string message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
                                   
            //await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: string.Empty, routingKey: queueName, mandatory: false, body: body);

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
