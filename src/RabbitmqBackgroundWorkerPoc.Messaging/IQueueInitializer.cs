namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public interface IQueueInitializer
    {
        Task EnsureQueueAsync();
    }
}
