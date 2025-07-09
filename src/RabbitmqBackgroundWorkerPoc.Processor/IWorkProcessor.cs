namespace RabbitmqBackgroundWorkerPoc.Processor
{
    public interface IWorkProcessor
    {
        Task<string> ProcessAsync(string message,CancellationToken cancellationToken);
    }
}
