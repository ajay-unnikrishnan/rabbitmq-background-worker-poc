using RabbitmqBackgroundWorkerPoc.Api.Business;
using RabbitmqBackgroundWorkerPoc.Messaging;


namespace RabbitmqBackgroundWorker.Api.Business
{
    public class MessagePublisherService: IMessagePublisherService
    {
        private readonly IMessagePublisher _messagePublisher;
        public MessagePublisherService(IMessagePublisher messagePublisher)
        {
            _messagePublisher = messagePublisher;
        }
        public async Task PublishMessageAsync(string userMessage)
        {
            var message = new QueueMessage
            {
                Id = Guid.NewGuid(),
                Text = userMessage
            };
            await _messagePublisher.PublishAsync("test-queue", message);
        }
    }
}
