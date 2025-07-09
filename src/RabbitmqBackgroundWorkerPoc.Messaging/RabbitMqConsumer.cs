using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqConsumer : IMessageQueueConsumer
    {
        private readonly ConnectionFactory _factory;
        public RabbitMqConsumer(IConfiguration config)
        {
            var uri = config.GetConnectionString("RabbitMQ") ?? throw new Exception("RabbitMQ connection string missing");
            _factory = new ConnectionFactory
            {
                Uri = new Uri(uri),
                ConsumerDispatchConcurrency = 1
            };
        }
        public async Task StartListeningAsync(string queueName, Func<string, CancellationToken, Task> onMesssage, CancellationToken stoppingToken)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await onMesssage(message, stoppingToken);
                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch
                {
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
                await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            };

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
