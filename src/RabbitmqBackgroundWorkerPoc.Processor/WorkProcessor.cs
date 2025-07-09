
namespace RabbitmqBackgroundWorkerPoc.Processor
{
    public class WorkProcessor: IWorkProcessor
    {
        public async Task<string> ProcessAsync(string message, CancellationToken cancellationToken)
        {
            await Task.Delay(10000, cancellationToken);
            Console.WriteLine($"[Processor] Received message : { message }");
            return "Work Processor is alive";
        }
    }
}
