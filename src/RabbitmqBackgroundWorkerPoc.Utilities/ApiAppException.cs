using Microsoft.AspNetCore.Http;

namespace RabbitmqBackgroundWorkerPoc.Utilities
{
    public class ApiAppException : Exception
    {
        public int StatusCode { get; }

        public ApiAppException(string message, int statusCode = StatusCodes.Status400BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
