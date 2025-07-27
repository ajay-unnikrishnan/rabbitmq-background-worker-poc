using RabbitmqBackgroundWorkerPoc.Api.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RabbitmqBackgroundWorkerPoc.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessagePublisherService _messagePublisherService;
        private readonly ILogger<MessageController> _logger;
        public MessageController(ILogger<MessageController> logger, IMessagePublisherService messagePublisherService)
        {
            _logger = logger;
            _messagePublisherService = messagePublisherService;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            _logger.LogInformation("Received message at API: {Message}", message);
            await _messagePublisherService.PublishMessageAsync(message);
            return Ok("Message Published");
        }
    }
}
