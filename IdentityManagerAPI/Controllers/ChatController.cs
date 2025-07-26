using DataAcess.Repos.IRepos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityManagerAPI.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : Controller
    {
        private readonly IChatRepository chatRepo;

        public ChatController(IChatRepository _chatRepo)
        {
            chatRepo = _chatRepo;
        }
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] string userId, [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null)
                return BadRequest("user must be logged");
            var messaages = await chatRepo.GetMessagesAsync(currentUserId, userId, page, pagesize);
            return Ok(messaages);
        }
        [HttpPost("mark-delivered")]
        public async Task<IActionResult> MarkAsDelivered([FromBody] List<int> messageIds)
        {
            await chatRepo.MarkMessagesAsDeliveredAsync(messageIds);
            return NoContent();
        }
    }
}
