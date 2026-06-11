using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Data;
using System.Security.Claims;

namespace StudyVerse.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateTime.Today;

            ViewBag.TotalTasks = await _context.TaskItems.CountAsync(t => t.UserId == userId);
            ViewBag.CompletedTasks = await _context.TaskItems.CountAsync(t => t.UserId == userId && t.IsCompleted);
            ViewBag.PendingTasks = await _context.TaskItems.CountAsync(t => t.UserId == userId && !t.IsCompleted);

            ViewBag.TodayTasks = await _context.TaskItems
                .Where(t => t.UserId == userId && t.DueDate.Date == today)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .ToListAsync();

            ViewBag.UpcomingTasks = await _context.TaskItems
                .Where(t => t.UserId == userId && t.DueDate.Date > today)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentDecks = await _context.FlashcardDecks
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(3)
                .ToListAsync();

            ViewBag.RecentPosts = await _context.ForumPosts
                .OrderByDescending(p => p.CreatedAt)
                .Take(3)
                .ToListAsync();

            return View();
        }
    }
}