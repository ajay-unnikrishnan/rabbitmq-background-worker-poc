using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqQueueInitializer : IQueueInitializer
    {
        private readonly ConnectionFactory _factory;
        private readonly MessagingSettings _settings;
        public RabbitMqQueueInitializer(IConfiguration config, IOptions<MessagingSettings> options)
        {
            _settings = options.Value;
            
            _factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.RabbitMqUri)                
            };            
        }
        // EnsureQueueAsync is called only once in API startup to avoid declaring the queue on every publish.
        // In the worker, the queue is declared inside the consumer (StartListeningAsync) which is a one-time task
        // and it ensures the queue exists before consumption.
        public async Task EnsureQueueAsync()
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false);            

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
