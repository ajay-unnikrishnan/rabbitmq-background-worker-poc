namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IQueueInitializer
    {
        Task EnsureQueue(string queueName);
    }
}
