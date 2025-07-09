using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class QueueInitializer : IQueueInitializer
    {
        private readonly ConnectionFactory _factory;
        public QueueInitializer(IConfiguration config)
        {
            var uri = config.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ connection string missing");
            _factory = new ConnectionFactory
            {
                Uri = new Uri(uri)                
            };
        }
        public async Task EnsureQueue(string queueName)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);            

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
