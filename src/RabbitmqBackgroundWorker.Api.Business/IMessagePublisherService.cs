namespace RabbitmqBackgroundWorkerPoc.Api.Business
{
    public interface IMessagePublisherService
    {
        Task PublishMessageAsync(string userMessage);
    }
}
