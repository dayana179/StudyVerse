using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;

namespace StudyVerse.Controllers.Api
{
    [Route("api/chat")]
    [ApiController]
    public class ChatApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChatApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            var messages = await _context.ChatMessages
                .Include(m => m.User)
                .OrderBy(m => m.SentAt)
                .Take(100)
                .Select(m => new
                {
                    m.Id,
                    m.Message,
                    m.UserId,
                    UserName = m.User != null ? m.User.UserName : "User",
                    SentAt = m.SentAt,
                    m.ForumPostId,
                    m.ForumPostTitle
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MobileChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Message is required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            var lastMessage = await _context.ChatMessages
                .Where(m => m.UserId == request.UserId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            if (lastMessage != null && (DateTime.Now - lastMessage.SentAt).TotalSeconds < 10)
            {
                return BadRequest("Please wait before sending another message.");
            }

            var chatMessage = new ChatMessage
            {
                UserId = request.UserId,
                Message = request.Message.Trim(),
                SentAt = DateTime.Now,
                ForumPostId = request.ForumPostId,
                ForumPostTitle = request.ForumPostTitle
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                chatMessage.Id,
                chatMessage.Message,
                chatMessage.UserId,
                UserName = user.UserName,
                SentAt = chatMessage.SentAt,
                chatMessage.ForumPostId,
                chatMessage.ForumPostTitle
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var message = await _context.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (message == null)
            {
                return NotFound();
            }

            _context.ChatMessages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class MobileChatRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public int? ForumPostId { get; set; }

        public string? ForumPostTitle { get; set; }
    }
}