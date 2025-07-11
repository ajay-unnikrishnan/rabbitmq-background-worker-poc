using System.ComponentModel.DataAnnotations;

namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class MessagingSettings
    {
        [Required]
        public string RabbitMqUri { get; set; } = string.Empty;
        [Required]
        public string QueueName { get; set; } = string.Empty;
    }
}
