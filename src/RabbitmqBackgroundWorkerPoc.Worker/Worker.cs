using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;

namespace RabbitmqBackgroundWorkerPoc.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IQueueConsumer _consumer;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IQueueConsumer consumer)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {       
        await _consumer.StartListeningAsync(async (message, token) =>
        {            
            using (var scope = _scopeFactory.CreateScope())
            {
                var processor = scope.ServiceProvider.GetRequiredService<IWorkProcessor>();                
                await processor.ProcessAsync(message.Text, stoppingToken);

                _logger.LogInformation("Ccompleted the process at: {time}", DateTimeOffset.Now);
            }
        }, stoppingToken);
    }
}
