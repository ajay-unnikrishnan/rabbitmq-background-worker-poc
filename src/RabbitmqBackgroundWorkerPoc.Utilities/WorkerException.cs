using Microsoft.AspNetCore.Http;

namespace RabbitmqBackgroundWorkerPoc.Utilities
{
    public class WorkerException : Exception
    {
        public int StatusCode { get; }

        public WorkerException(string message, int statusCode = StatusCodes.Status500InternalServerError)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
