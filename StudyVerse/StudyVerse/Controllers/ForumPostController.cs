using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Data;
using StudyVerse.Models;
using System.Security.Claims;

namespace StudyVerse.Controllers
{
    [Authorize]
    public class ForumPostController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumPostController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.ForumPosts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForumPost forumPost)
        {
            if (ModelState.IsValid)
            {
                forumPost.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                forumPost.CreatedAt = DateTime.Now;

                _context.Add(forumPost);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(forumPost);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post == null) return NotFound();

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ForumPost forumPost)
        {
            if (id != forumPost.Id) return NotFound();

            if (ModelState.IsValid)
            {
                forumPost.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                forumPost.CreatedAt = DateTime.Now;

                _context.Update(forumPost);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(forumPost);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post == null) return NotFound();

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post != null)
            {
                _context.ForumPosts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}