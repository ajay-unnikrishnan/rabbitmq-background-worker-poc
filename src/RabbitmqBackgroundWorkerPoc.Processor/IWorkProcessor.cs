namespace RabbitmqBackgroundWorkerPoc.Processor
{
    public interface IWorkProcessor
    {
        Task ProcessAsync(string message,CancellationToken cancellationToken);
    }
}
