using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;
using RabbitmqBackgroundWorkerPoc.Utilities;
using Serilog.Context;

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
        _logger.LogInformation("Worker started listening to the queue");
        await _consumer.StartListeningAsync(async (message, token) =>
        {
            using (LogContext.PushProperty("ProcessId", message.ProcessId))
            {
                try
                {
                    var scope = _scopeFactory.CreateScope();
               
                    var processor = scope.ServiceProvider.GetRequiredService<IWorkProcessor>();

                    _logger.LogInformation("Started processing message with ProcessId: {ProcessId}", message.ProcessId.ToString());
                    
                    await processor.ProcessAsync(message.Text, stoppingToken);

                    _logger.LogInformation("Completed processing message with ProcessId: {ProcessId}", message.ProcessId.ToString());
                }
                catch (WorkerException ex)
                {
                    _logger.LogError(ex, "{ErrorMessage} | ProcessId: {ProcessId}", ex.Message, message.ProcessId.ToString());                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred while processing message with ID {ProcessId}", message.ProcessId.ToString());                    
                }
            }
        }, stoppingToken);
        
    }
}
