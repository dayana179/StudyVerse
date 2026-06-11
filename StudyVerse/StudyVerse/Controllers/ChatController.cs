using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Data;
using StudyVerse.Models;
using System.Security.Claims;

namespace StudyVerse.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var messages = await _context.ChatMessages
            .Include(m => m.User)
            .OrderBy(m => m.SentAt)
            .Take(100)
            .ToListAsync();

            ViewBag.ExistingForumPostIds = await _context.ForumPosts
                .Select(p => p.Id)
                .ToListAsync();
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(string message)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var lastMessage = await _context.ChatMessages
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.SentAt)
                .FirstOrDefaultAsync();

            if (lastMessage != null && (DateTime.Now - lastMessage.SentAt).TotalSeconds < 10)
            {
                TempData["ChatError"] = "Please wait before sending another message.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                var chatMessage = new ChatMessage
                {
                    Message = message,
                    UserId = userId,
                    SentAt = DateTime.Now
                };

                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DiscussForumPost(int id)
        {
            var post = await _context.ForumPosts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chatMessage = new ChatMessage
            {
                Message = "Started a discussion from forum post:",
                UserId = userId,
                SentAt = DateTime.Now,
                ForumPostId = post.Id,
                ForumPostTitle = post.Title
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = await _context.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (message == null)
            {
                TempData["ChatError"] = "You can only delete your own messages.";
                return RedirectToAction(nameof(Index));
            }

            _context.ChatMessages.Remove(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}