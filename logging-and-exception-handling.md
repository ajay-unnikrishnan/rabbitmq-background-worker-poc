# Logging and Exception Handling Setup

This project uses **Serilog** as the primary logging framework. This document explains the setup of **Serilog logging**, **exception handling**, and **structured log output** in both the API and Worker services of the project.

## 📦 NuGet Packages Used

Install these packages in both **API** and **Worker** projects:

```bash
dotnet add package Serilog
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.MSSqlServer
dotnet add package Serilog.Sinks.Console
```
---

## 🛠 SQL Table Creation (Auto-Create Enabled)

We use:

- `AutoCreateSqlTable = true`
- A custom table name, e.g., `AppLogs`

Serilog will automatically create the table when logging starts.

> ⚠️ If `TableName` is **not explicitly set**, it will default to `Logs`, even with `AutoCreateSqlTable = true`.


### 🧱 Table Schema Created by Serilog

| Column          | Type              |
|------------------|-------------------|
| `Id`            | `int IDENTITY PK` |
| `Message`       | `nvarchar(max)`   |
| `MessageTemplate` | `nvarchar(max)` |
| `Level`         | `nvarchar(128)`   |
| `TimeStamp`     | `datetimeoffset`  |
| `Exception`     | `nvarchar(max)`   |
| `Properties`    | `nvarchar(max)`   |
| `LogEvent`      | `nvarchar(max)`   |
| `ProcessId`     | `nvarchar(100)` _(custom column)_ ✅

---

## ⚙️ Serilog Configuration in Program.cs

### 🔹 For API

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.MSSqlServer(
        connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "AppLogs",
            AutoCreateSqlTable = true
        },
        columnOptions: ColumnOptionsFactory.Create())
    .CreateLogger();
```

### 🔹 ColumnOptionsFactory.cs

```csharp
public static class ColumnOptionsFactory
{
    public static ColumnOptions Create()
    {
        return new ColumnOptions
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
    }
}
```
---

### 🧩 Serilog `LogContext.PushProperty()` – Explained

✅ What Is It?

`LogContext.PushProperty()` is a Serilog method used to **enrich log events with custom properties** that apply only within a specific scope (usually a request, message, or operation).

It allows you to **add structured, contextual data** — like `ProcessId`, or `UserId` — to every log written within a `using` block.

### 🧪 Example Usage

```csharp
using (LogContext.PushProperty("ProcessId", message.Id))
{
    _logger.LogInformation("Started processing message with id: {ProcessId}", ProcessId);
    // All logs here will include "ProcessId"
}
```

### 🧠 Why Use It?

| Feature                         | Benefit                                                                 |
|----------------------------------|-------------------------------------------------------------------------|
| Structured logs                 | Adds key-value pairs like `"ProcessId": "abc-123"` to each log entry   |
| Scoped enrichment               | Only applies to logs within the `using` block                          |
| Works with sinks                | Included in `Properties` JSON, database columns, console, and file logs |
| Required for custom DB columns | Automatically populates mapped SQL columns like `ProcessId` 

---

## 🛡️ Global Exception Middleware (`GlobalExceptionMiddleware.cs`)

This middleware handles **application-wide unhandled exceptions** and returns a consistent JSON error response while logging all exceptions via Serilog.


### 📄 File Location
```plaintext
RabbitmqBackgroundWorkerPoc.Api/GlobalExceptionMiddleware.cs
```

### 🚀 How to Register Global Exception Middleware in `Program.cs`

To enable centralized exception handling, add the middleware to your request pipeline in `Program.cs` **before** `UseHttpsRedirection()`:

```csharp
app.UseMiddleware<GlobalExceptionMiddleware>();
```

### ✅ Benefits of This Middleware

| Feature                 | Benefit                                                    |
|-------------------------|-------------------------------------------------------------|
| Centralized error logic | No need for try-catch in every controller                  |
| Structured logs         | Serilog captures all exception details automatically       |
| JSON responses          | Ensures client gets a consistent and clean error format     |
| Custom status codes     | Supports `ApiAppException` for user-defined HTTP responses |


### 💡 Sample JSON Responses

### 🔹 Known exception (`ApiAppException`)

```json
{
  "message": "Invalid input provided"
}
```

### 🔹 Unhandled exception
``` json
{
  "message": "Internal Server Error"
}
```
