using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;
using RabbitmqBackgroundWorkerPoc.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services
    .AddOptions<MessagingSettings>()
    .Bind(builder.Configuration.GetSection("Messaging"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IWorkProcessor, WorkProcessor>();
builder.Services.AddSingleton<IQueueConsumer, RabbitMqConsumer>();

var host = builder.Build();
host.Run();
