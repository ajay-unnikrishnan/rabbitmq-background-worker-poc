namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IQueueConsumer
    {
        Task StartListeningAsync(Func<QueueMessage, CancellationToken, Task> onMesssage, CancellationToken stoppingToken);
    }
}
