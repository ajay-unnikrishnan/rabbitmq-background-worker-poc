namespace RabbitmqBackgroundWorkerPoc.Messaging
{
    public class QueueMessage
    {
        public Guid ProcessId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
