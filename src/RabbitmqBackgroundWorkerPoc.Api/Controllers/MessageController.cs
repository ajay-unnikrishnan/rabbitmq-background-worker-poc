using RabbitmqBackgroundWorkerPoc.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RabbitmqBackgroundWorkerPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageQueueClient _queueClient;
        public MessageController(IMessageQueueClient queueClient)
        {
            _queueClient = queueClient;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            await _queueClient.PublishAsync("test-queue", message);
            return Ok("Message Published");
        }
    }
}
