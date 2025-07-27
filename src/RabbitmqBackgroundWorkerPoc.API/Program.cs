using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Api.Business;
using Serilog.Sinks.MSSqlServer;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");

#region Serilog;

Log.Logger = new LoggerConfiguration()    
    .WriteTo.MSSqlServer(
        connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "AppLogs",
            AutoCreateSqlTable = false
        })
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

#endregion

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services
    .AddOptions<MessagingSettings>()
    .Bind(builder.Configuration.GetSection("Messaging"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IQueueInitializer, RabbitMqQueueInitializer>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IMessagePublisherService, MessagePublisherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{    
    var queueInitializer = scope.ServiceProvider.GetRequiredService<IQueueInitializer>();    
    await queueInitializer.EnsureQueueAsync();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


///////////////////////////////////
///
//dotnet add package Serilog
//dotnet add package Serilog.Sinks.MSSqlServer
//dotnet add package Serilog.Sinks.Console
//dotnet add package Serilog.AspNetCore

//CREATE TABLE [dbo].[Logs] (
//    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
//    [Message] NVARCHAR(MAX) NULL,
//    [MessageTemplate] NVARCHAR(MAX) NULL,
//    [Level] NVARCHAR(128) NULL,
//    [TimeStamp] DATETIMEOFFSET NOT NULL,
//    [Exception] NVARCHAR(MAX) NULL,
//    [Properties] NVARCHAR(MAX) NULL,
//    [LogEvent] NVARCHAR(MAX) NULL,
//    [ProcessId] NVARCHAR(100) NULL -- Custom column for the process ID
//);