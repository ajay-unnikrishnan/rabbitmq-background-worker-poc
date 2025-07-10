namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IMessageQueueConsumer
    {
        Task StartListeningAsync(string queueName, Func<QueueMessage, CancellationToken, Task> onMesssage, CancellationToken stoppingToken);
    }
}
