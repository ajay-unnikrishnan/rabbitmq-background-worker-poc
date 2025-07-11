using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class RabbitMqConsumer : IQueueConsumer
    {
        private readonly ConnectionFactory _factory;
        private readonly MessagingSettings _settings;
        public RabbitMqConsumer(IConfiguration config, IOptions<MessagingSettings> options)
        {
            _settings = options.Value;

            _factory = new ConnectionFactory
            {
                Uri = new Uri(_settings.RabbitMqUri)
            };
        }
        public async Task StartListeningAsync(Func<QueueMessage, CancellationToken, Task> onMesssage, CancellationToken stoppingToken)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _settings.QueueName, durable: true, exclusive: false, autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, deliveryEventArgs) =>
            {
                var json = Encoding.UTF8.GetString(deliveryEventArgs.Body.ToArray());

                try
                {
                    var message = JsonSerializer.Deserialize<QueueMessage>(json) ?? throw new Exception("Invalid message format");
                    await onMesssage(message, stoppingToken);
                    await channel.BasicAckAsync(deliveryEventArgs.DeliveryTag, multiple: false);
                }
                catch
                {
                    await channel.BasicNackAsync(deliveryEventArgs.DeliveryTag, multiple: false, requeue: true);
                }
            };
            await channel.BasicConsumeAsync(queue: _settings.QueueName, autoAck: false, consumer: consumer);
                       
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await channel.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
