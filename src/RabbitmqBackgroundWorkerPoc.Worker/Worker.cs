using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;

namespace RabbitmqBackgroundWorkerPoc.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMessageQueueConsumer _consumer;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IMessageQueueConsumer consumer)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExecuteAsync");
        await _consumer.StartListeningAsync("test-queue", async (message, token) =>
        {
            _logger.LogInformation("Executeing callback");
            using (var scope = _scopeFactory.CreateScope())
            {
                var processor = scope.ServiceProvider.GetRequiredService<IWorkProcessor>();
                var status = await processor.ProcessAsync(message, stoppingToken);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation(status);
            }
        }, stoppingToken);
    }
}
