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
        public MessageController(IMessagePublisherService messagePublisherService)
        {
            _messagePublisherService = messagePublisherService;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string message)
        {
            await _messagePublisherService.PublishMessageAsync(message);
            return Ok("Message Published");
        }
    }
}
