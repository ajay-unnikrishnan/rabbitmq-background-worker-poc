using RabbitmqBackgroundWorkerPoc.Messaging;
using RabbitmqBackgroundWorkerPoc.Api.Business;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using RabbitmqBackgroundWorkerPoc.Api;
using RabbitmqBackgroundWorkerPoc.Utilities;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DBConnection");

#region Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) //Only log Warning or above for logs from the Microsoft.* namespaces
    .MinimumLevel.Override("Microsoft.AspNetCore.HttpsPolicy", Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "AppLogs",
            AutoCreateSqlTable = false
        },
        columnOptions: ColumnOptionsFactory.Create())
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

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting API host...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


