
namespace RabbitmqBackgroundWorkerPoc.Processor
{
    public class WorkProcessor: IWorkProcessor
    {
        public async Task ProcessAsync(string message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[Processor] Received message : { message }");
            await Task.Delay(10000, cancellationToken);
            Console.WriteLine($"[Processor] Finished processing message : { message }");
        }
    }
}
