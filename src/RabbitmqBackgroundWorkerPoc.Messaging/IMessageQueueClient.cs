namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IQueueInitializer
    {
        Task EnsureQueue(string queueName);
    }
    public interface IMessageQueueClient
    {
        Task PublishAsync(string queueName, string message);
    }    
}
