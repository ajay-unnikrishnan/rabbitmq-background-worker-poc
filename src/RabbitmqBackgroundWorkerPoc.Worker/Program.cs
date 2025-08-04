using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Processor;
using RabbitmqBackgroundWorkerPoc.Worker;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");

#region Serilog;

var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new SqlColumn("ProcessId", SqlDbType.NVarChar, dataLength: 100)
    },    
    Store = new Collection<StandardColumn>
    {
        StandardColumn.Message,
        StandardColumn.MessageTemplate,
        StandardColumn.Level,
        StandardColumn.TimeStamp,
        StandardColumn.Exception,
        StandardColumn.Properties,
        StandardColumn.LogEvent
    }
};

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.MSSqlServer(
        connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "AppLogs",
            AutoCreateSqlTable = false
        },
        columnOptions: columnOptions)
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
host.Run();
