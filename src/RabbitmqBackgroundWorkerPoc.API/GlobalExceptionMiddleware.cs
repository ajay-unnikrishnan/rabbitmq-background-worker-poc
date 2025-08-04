using RabbitmqBackgroundWorkerPoc.Utilities;
using System.Text.Json;

namespace RabbitmqBackgroundWorkerPoc.Api
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {  
                context.Response.ContentType = "application/json";
                object errorMessage;
                int statusCode;                                

                if (ex is ApiAppException apiEx)
                {
                    _logger.LogError(ex, ex.Message);
                    errorMessage = new { message = apiEx.Message };
                    statusCode = apiEx.StatusCode;
                }
                else
                {
                    _logger.LogError(ex, "An unhandled exception occurred.");
                    errorMessage = new { message = "Internal Server Error" };
                    statusCode = StatusCodes.Status500InternalServerError;
                }

                context.Response.StatusCode = statusCode;
                var errorResponse = JsonSerializer.Serialize(errorMessage);
                await context.Response.WriteAsync(errorResponse);
            }
        }
    }
}
