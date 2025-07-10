namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class QueueMessage
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
