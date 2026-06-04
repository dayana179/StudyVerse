
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;
using StudyVerse.Data;

public class TaskItemsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TaskItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: TASKITEMS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.TaskItems.ToListAsync());
    }

    // GET: TASKITEMS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var taskitem = await _context.TaskItems
            .FirstOrDefaultAsync(m => m.Id == id);
        if (taskitem == null)
        {
            return NotFound();
        }

        return View(taskitem);
    }

    // GET: TASKITEMS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: TASKITEMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,IsCompleted,Priority,UserId,User")] TaskItem taskitem)
    {
        if (ModelState.IsValid)
        {
            _context.Add(taskitem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(taskitem);
    }

    // GET: TASKITEMS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var taskitem = await _context.TaskItems.FindAsync(id);
        if (taskitem == null)
        {
            return NotFound();
        }
        return View(taskitem);
    }

    // POST: TASKITEMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Description,DueDate,IsCompleted,Priority,UserId,User")] TaskItem taskitem)
    {
        if (id != taskitem.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(taskitem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskItemExists(taskitem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(taskitem);
    }

    // GET: TASKITEMS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var taskitem = await _context.TaskItems
            .FirstOrDefaultAsync(m => m.Id == id);
        if (taskitem == null)
        {
            return NotFound();
        }

        return View(taskitem);
    }

    // POST: TASKITEMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var taskitem = await _context.TaskItems.FindAsync(id);
        if (taskitem != null)
        {
            _context.TaskItems.Remove(taskitem);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TaskItemExists(int? id)
    {
        return _context.TaskItems.Any(e => e.Id == id);
    }
}
