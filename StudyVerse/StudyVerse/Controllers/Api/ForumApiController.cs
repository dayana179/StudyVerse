using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Data;

namespace StudyVerse.Controllers.Api
{
    [Route("api/forum")]
    [ApiController]
    //[Authorize]
    public class ForumApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ForumApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.ForumPosts
                .Include(p => p.Attachments)
                .Include(p => p.Replies)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Category,
                    p.CreatedAt,
                    AttachmentPath = p.AttachmentPath,
                    AttachmentFileName = p.AttachmentFileName,
                    Attachments = p.Attachments.Select(a => new
                    {
                        a.AttachmentId,
                        a.FileName,
                        a.FilePath,
                        a.FileSize
                    }),
                    Replies = p.Replies.Select(r => new
                    {
                        r.Id,
                        r.Content,
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
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.Category,
                    p.CreatedAt,
                    AttachmentPath = p.AttachmentPath,
                    AttachmentFileName = p.AttachmentFileName,
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
    }
}