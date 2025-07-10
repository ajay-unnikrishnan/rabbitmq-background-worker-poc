namespace RabbitmqBackgroundWorkerPoc.Messaging
{    
    public interface IMessagePublisher
    {
        Task PublishAsync(string queueName, QueueMessage message);
    }    
}
