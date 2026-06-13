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
                .Include(p => p.Attachments)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.ForumPosts
                .Include(p => p.Attachments)
                .Include(p => p.Replies)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            if (post.Replies != null)
            {
                post.Replies = post.Replies
                    .OrderBy(r => r.CreatedAt)
                    .ToList();
            }

            return View(post);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ForumPost forumPost, List<IFormFile> attachments)
        {
            if (ModelState.IsValid)
            {
                forumPost.CreatedAt = DateTime.Now;
                forumPost.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.ForumPosts.Add(forumPost);
                await _context.SaveChangesAsync();

                if (attachments != null && attachments.Count > 0)
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "forum-attachments");

                    if (!Directory.Exists(uploadFolder))
                    {
                        Directory.CreateDirectory(uploadFolder);
                    }

                    foreach (var file in attachments)
                    {
                        if (file != null && file.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                            string filePath = Path.Combine(uploadFolder, uniqueFileName);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            var attachment = new ForumAttachment
                            {
                                ForumPostId = forumPost.Id,
                                FileName = file.FileName,
                                FilePath = "/forum-attachments/" + uniqueFileName,
                                ContentType = file.ContentType,
                                FileSize = file.Length
                            };

                            _context.ForumAttachments.Add(attachment);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(int forumPostId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction(nameof(Details), new { id = forumPostId });
            }

            var postExists = await _context.ForumPosts.AnyAsync(p => p.Id == forumPostId);

            if (!postExists)
            {
                return NotFound();
            }

            var reply = new ForumReply
            {
                ForumPostId = forumPostId,
                Content = content.Trim(),
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = User.Identity?.Name ?? "User",
                CreatedAt = DateTime.Now
            };

            _context.ForumReplies.Add(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = forumPostId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReply(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var reply = await _context.ForumReplies
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reply == null)
            {
                return NotFound();
            }

            int forumPostId = reply.ForumPostId;

            _context.ForumReplies.Remove(reply);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = forumPostId });
        }
    }
}