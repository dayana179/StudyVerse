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
        private readonly IWebHostEnvironment _environment;

        public ForumPostController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public async Task<IActionResult> Create(ForumPost forumPost, IFormFile? attachment)
        {
            if (ModelState.IsValid)
            {
                forumPost.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                forumPost.CreatedAt = DateTime.Now;

                if (attachment != null && attachment.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "forum");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(attachment.FileName);
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(fileStream);
                    }

                    forumPost.AttachmentFileName = attachment.FileName;
                    forumPost.AttachmentPath = "/uploads/forum/" + uniqueFileName;
                }

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
        public async Task<IActionResult> Edit(int id, ForumPost forumPost, IFormFile? attachment)
        {
            if (id != forumPost.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var existingPost = await _context.ForumPosts
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

                if (existingPost == null) return NotFound();

                existingPost.Title = forumPost.Title;
                existingPost.Content = forumPost.Content;
                existingPost.Category = forumPost.Category;

                if (attachment != null && attachment.Length > 0)
                {
                    string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "forum");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(attachment.FileName);
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(fileStream);
                    }

                    existingPost.AttachmentFileName = attachment.FileName;
                    existingPost.AttachmentPath = "/uploads/forum/" + uniqueFileName;
                }

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
                if (!string.IsNullOrEmpty(post.AttachmentPath))
                {
                    string oldFilePath = Path.Combine(
                        _environment.WebRootPath,
                        post.AttachmentPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                _context.ForumPosts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}