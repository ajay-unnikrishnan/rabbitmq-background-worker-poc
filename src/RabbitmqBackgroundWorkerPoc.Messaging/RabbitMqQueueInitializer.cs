using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqQueueInitializer : IQueueInitializer
    {
        private readonly ConnectionFactory _factory;
        public RabbitMqQueueInitializer(IConfiguration config)
        {
            var uri = config.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ connection string missing");
            _factory = new ConnectionFactory
            {
                Uri = new Uri(uri)                
            };
        }
        // EnsureQueueAsync is called only once in API startup to avoid declaring the queue on every publish.
        // In the worker, the queue is declared inside the consumer (StartListeningAsync) which is a one-time task
        // and it ensures the queue exists before consumption.
        public async Task EnsureQueueAsync(string queueName)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);            

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
