namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IQueueConsumer
    {
        Task StartListeningAsync(string queueName, Func<QueueMessage, CancellationToken, Task> onMesssage, CancellationToken stoppingToken);
    }
}
