using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;
using System.Security.Claims;

namespace StudyVerse.Controllers
{
    [Authorize]
    public class FlashcardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlashcardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var decks = await _context.FlashcardDecks
                .Where(d => d.UserId == userId)
                .Include(d => d.Flashcards)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(decks);
        }

        public IActionResult CreateDeck()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeck(FlashcardDeck deck)
        {
            if (ModelState.IsValid)
            {
                deck.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                deck.CreatedAt = DateTime.Now;

                _context.FlashcardDecks.Add(deck);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(deck);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.FlashcardDecks
                .Include(d => d.Flashcards)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck == null) return NotFound();

            return View(deck);
        }

        public async Task<IActionResult> EditDeck(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.FlashcardDecks
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck == null) return NotFound();

            return View(deck);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeck(int id, FlashcardDeck deck)
        {
            if (id != deck.Id) return NotFound();

            if (ModelState.IsValid)
            {
                deck.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Update(deck);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(deck);
        }

        public async Task<IActionResult> DeleteDeck(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.FlashcardDecks
                .Include(d => d.Flashcards)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck == null) return NotFound();

            return View(deck);
        }

        [HttpPost, ActionName("DeleteDeck")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeckConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deck = await _context.FlashcardDecks
                .Include(d => d.Flashcards)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (deck != null)
            {
                _context.FlashcardDecks.Remove(deck);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateCard(int deckId)
        {
            ViewBag.DeckId = deckId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(Flashcard flashcard)
        {
            if (ModelState.IsValid)
            {
                _context.Flashcards.Add(flashcard);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = flashcard.FlashcardDeckId });
            }

            ViewBag.DeckId = flashcard.FlashcardDeckId;
            return View(flashcard);
        }

        public async Task<IActionResult> DeleteCard(int? id)
        {
            if (id == null) return NotFound();

            var card = await _context.Flashcards
                .FirstOrDefaultAsync(c => c.Id == id);

            if (card == null) return NotFound();

            int deckId = card.FlashcardDeckId;

            _context.Flashcards.Remove(card);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = deckId });
        }
    }
}