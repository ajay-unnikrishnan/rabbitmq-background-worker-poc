namespace RabbitmqBackgroundWorkerPoc.Messaging
{    
    public interface IMessagePublisher
    {
        Task PublishAsync(QueueMessage message);
    }    
}
