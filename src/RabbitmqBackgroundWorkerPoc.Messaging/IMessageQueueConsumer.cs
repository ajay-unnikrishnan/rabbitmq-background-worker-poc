namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IMessageQueueConsumer
    {
        Task StartListeningAsync(string queueName, Func<string, CancellationToken, Task> onMesssage, CancellationToken stoppingToken);
    }
}
