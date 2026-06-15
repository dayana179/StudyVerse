using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;

namespace StudyVerse.Controllers.Api
{
    [Route("api/forum")]
    [ApiController]
    public class ForumApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ForumApiController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.ForumPosts
                .Include(p => p.Attachments)
                .Include(p => p.Replies)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Category,
                    p.CreatedAt,
                    p.UserId,
                    UserName = p.User != null ? p.User.UserName : "User",
                    p.AttachmentPath,
                    p.AttachmentFileName,
                    Attachments = p.Attachments.Select(a => new
                    {
                        a.AttachmentId,
                        a.FileName,
                        a.FilePath,
                        a.FileSize
                    }),
                    Replies = p.Replies
                        .OrderBy(r => r.CreatedAt)
                        .Select(r => new
                        {
                            r.Id,
                            r.Content,
                            r.UserId,
                            r.UserName,
                            r.CreatedAt
                        })
                })
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("posts/{id}")]
        public async Task<IActionResult> GetPostDetails(int id)
        {
            var post = await _context.ForumPosts
                .Include(p => p.Attachments)
                .Include(p => p.Replies)
                .Include(p => p.User)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Category,
                    p.CreatedAt,
                    p.UserId,
                    UserName = p.User != null ? p.User.UserName : "User",
                    p.AttachmentPath,
                    p.AttachmentFileName,
                    Attachments = p.Attachments.Select(a => new
                    {
                        a.AttachmentId,
                        a.FileName,
                        a.FilePath,
                        a.FileSize
                    }),
                    Replies = p.Replies
                        .OrderBy(r => r.CreatedAt)
                        .Select(r => new
                        {
                            r.Id,
                            r.Content,
                            r.UserId,
                            r.UserName,
                            r.CreatedAt
                        })
                })
                .FirstOrDefaultAsync();

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost("posts")]
        public async Task<IActionResult> CreatePost([FromForm] MobileForumPostFormRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content is required.");
            }

            var post = new ForumPost
            {
                Title = request.Title.Trim(),
                Content = request.Content.Trim(),
                Category = request.Category ?? "General",
                UserId = request.UserId,
            };

            _context.ForumPosts.Add(post);
            await _context.SaveChangesAsync();

            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                await SaveMobileAttachmentsAsync(post.Id, request.Attachments);
            }

            return Ok(new
            {
                post.Id,
                post.Title,
                post.Content,
                post.Category,
                post.UserId
            });
        }

        [HttpPut("posts/{id}")]
        public async Task<IActionResult> EditPost(int id, [FromForm] MobileForumPostFormRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("User ID is required.");
            }

            var post = await _context.ForumPosts
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == request.UserId);

            if (post == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Content is required.");
            }

            post.Title = request.Title.Trim();
            post.Content = request.Content.Trim();
            post.Category = request.Category ?? "General";

            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                await SaveMobileAttachmentsAsync(post.Id, request.Attachments);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var post = await _context.ForumPosts
                .Include(p => p.Attachments)
                .Include(p => p.Replies)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (post == null)
            {
                return NotFound();
            }

            _context.ForumAttachments.RemoveRange(post.Attachments);
            _context.ForumReplies.RemoveRange(post.Replies);
            _context.ForumPosts.Remove(post);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("posts/{postId}/replies")]
        public async Task<IActionResult> AddReply(int postId, [FromBody] MobileForumReplyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Reply content is required.");
            }

            var post = await _context.ForumPosts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            var reply = new ForumReply
            {
                ForumPostId = postId,
                Content = request.Content.Trim(),
                UserId = request.UserId,
                UserName = user?.UserName ?? "User",
                CreatedAt = DateTime.Now
            };

            _context.ForumReplies.Add(reply);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                reply.Id,
                reply.Content,
                reply.UserId,
                reply.UserName,
                reply.CreatedAt
            });
        }

        [HttpDelete("replies/{replyId}")]
        public async Task<IActionResult> DeleteReply(int replyId, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var reply = await _context.ForumReplies
                .FirstOrDefaultAsync(r => r.Id == replyId && r.UserId == userId);

            if (reply == null)
            {
                return NotFound();
            }

            _context.ForumReplies.Remove(reply);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task SaveMobileAttachmentsAsync(int forumPostId, List<IFormFile> files)
        {
            string uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "forum");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                string originalFileName = Path.GetFileName(file.FileName);
                string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var attachment = new ForumAttachment
                {
                    ForumPostId = forumPostId,
                    FileName = originalFileName,
                    FilePath = $"/uploads/forum/{uniqueFileName}",
                    FileSize = file.Length
                };

                _context.ForumAttachments.Add(attachment);
            }

            await _context.SaveChangesAsync();
        }
    }

    public class MobileForumPostRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? Category { get; set; }
    }

    public class MobileForumPostFormRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? Category { get; set; }

        public List<IFormFile> Attachments { get; set; } = new();
    }

    public class MobileForumReplyRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }

   
    }