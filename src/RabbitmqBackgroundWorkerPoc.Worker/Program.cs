using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;
using RabbitmqBackgroundWorkerPoc.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<IWorkProcessor, WorkProcessor>();
builder.Services.AddSingleton<IMessageQueueConsumer, RabbitMqConsumer>();

var host = builder.Build();
host.Run();
