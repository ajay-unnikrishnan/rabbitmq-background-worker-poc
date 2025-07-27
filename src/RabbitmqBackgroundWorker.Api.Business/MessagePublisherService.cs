using RabbitmqBackgroundWorkerPoc.Api.Business;
using RabbitmqBackgroundWorkerPoc.Messaging;
using Microsoft.Extensions.Logging;


namespace RabbitmqBackgroundWorkerPoc.Api.Business
{
    public class MessagePublisherService: IMessagePublisherService
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<MessagePublisherService> _logger;
        public MessagePublisherService(IMessagePublisher messagePublisher, ILogger<MessagePublisherService> logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
        }
        public async Task PublishMessageAsync(string userMessage)
        {
            var message = new QueueMessage
            {
                ProcessId = Guid.NewGuid(),
                Text = userMessage
            };
            await _messagePublisher.PublishAsync(message);
            _logger.LogInformation("Message with ID {MessageId} published to queue", message.ProcessId);
        }
    }
}
