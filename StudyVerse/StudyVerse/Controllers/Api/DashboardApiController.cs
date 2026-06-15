using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;

namespace StudyVerse.Controllers.Api
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            int totalTasks = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .CountAsync();

            int completedTasks = await _context.TaskItems
                .Where(t => t.UserId == userId && t.IsCompleted)
                .CountAsync();

            int pendingTasks = await _context.TaskItems
                .Where(t => t.UserId == userId && !t.IsCompleted)
                .CountAsync();

            int progress = totalTasks == 0 ? 0 : completedTasks * 100 / totalTasks;

            var todayTasks = await _context.TaskItems
                .Where(t =>
                    t.UserId == userId &&
                    t.DueDate >= today &&
                    t.DueDate < tomorrow)
                .OrderBy(t => t.DueDate)
                .Take(4)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Priority,
                    t.DueDate
                })
                .ToListAsync();

            var upcomingTasks = await _context.TaskItems
                .Where(t =>
                    t.UserId == userId &&
                    t.DueDate >= tomorrow)
                .OrderBy(t => t.DueDate)
                .Take(4)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Priority,
                    t.DueDate
                })
                .ToListAsync();

            var recentDecks = await _context.FlashcardDecks
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(4)
                .Select(d => new
                {
                    d.Id,
                    d.Name
                })
                .ToListAsync();

            var recentPosts = await _context.ForumPosts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .Select(p => new
                {
                    p.Id,
                    p.Title
                })
                .ToListAsync();

            return Ok(new
            {
                totalTasks,
                completedTasks,
                pendingTasks,
                progress,
                todayTasks,
                upcomingTasks,
                recentDecks,
                recentPosts
            });
        }
    }
}