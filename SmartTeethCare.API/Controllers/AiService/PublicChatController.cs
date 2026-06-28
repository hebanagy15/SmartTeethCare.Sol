using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.AI_Services;
using SmartTeethCare.Core.Interfaces.Services.AiService;

namespace SmartTeethCare.API.Controllers.AiService
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicChatController : ControllerBase
    {
        private readonly IPublicChatService _service;

        public PublicChatController(IPublicChatService service)
        {
            _service = service;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat(PublicChatRequestDto dto)
        {
            var response = await _service.ChatAsync(dto);

            return Ok(response);
        }
    }
}
