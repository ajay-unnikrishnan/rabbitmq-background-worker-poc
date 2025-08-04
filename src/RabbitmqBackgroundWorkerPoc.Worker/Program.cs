using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;
using RabbitmqBackgroundWorkerPoc.Utilities;
using RabbitmqBackgroundWorkerPoc.Worker;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");

#region Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) //Only log Warning or above for logs from the Microsoft.* namespaces
    .MinimumLevel.Override("Microsoft.AspNetCore.HttpsPolicy", Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.MSSqlServer(
        connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "AppLogs",
            AutoCreateSqlTable = true
        },
        columnOptions: ColumnOptionsFactory.Create())
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

#endregion

builder.Services.AddHostedService<Worker>();

builder.Services
    .AddOptions<MessagingSettings>()
    .Bind(builder.Configuration.GetSection("Messaging"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IWorkProcessor, WorkProcessor>();
builder.Services.AddSingleton<IQueueConsumer, RabbitMqConsumer>();

var host = builder.Build();
try
{
    Log.Information("Starting Worker host...");
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
