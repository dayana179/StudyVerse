using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Data;
using StudyVerse.Models;
using System.Security.Claims;

namespace StudyVerse.Controllers.Api
{
    [Route("api/tasks")]
    [ApiController]
    //[Authorize]
    public class TaskApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tasks = await _context.TaskItems
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.IsCompleted)
                .ThenBy(t => t.DueDate)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.DueDate,
                    t.IsCompleted
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem taskItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            taskItem.UserId = userId;
            taskItem.IsCompleted = false;

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                taskItem.Id,
                taskItem.Title,
                taskItem.Description,
                taskItem.DueDate,
                taskItem.IsCompleted
            });
        }

        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> ToggleTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            task.IsCompleted = !task.IsCompleted;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                task.Id,
                task.Title,
                task.Description,
                task.DueDate,
                task.IsCompleted
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.TaskItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
            {
                return NotFound();
            }

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}