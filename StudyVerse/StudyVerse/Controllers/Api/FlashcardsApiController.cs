using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;

namespace StudyVerse.Controllers.Api
{
    [Route("api/flashcards")]
    [ApiController]
    public class FlashcardsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlashcardsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFlashcards([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var flashcards = await _context.Flashcards
                .Include(f => f.FlashcardDeck)
                .Where(f => f.FlashcardDeck != null && f.FlashcardDeck.UserId == userId)
                .OrderByDescending(f => f.Id)
                .Select(f => new
                {
                    f.Id,
                    f.Question,
                    f.Answer,
                    f.FlashcardDeckId,
                    DeckName = f.FlashcardDeck != null ? f.FlashcardDeck.Name : "Mobile Deck"
                })
                .ToListAsync();

            return Ok(flashcards);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlashcard([FromBody] MobileFlashcardRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Question is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Answer))
            {
                return BadRequest("Answer is required.");
            }

            var deck = await _context.FlashcardDecks
                .FirstOrDefaultAsync(d => d.UserId == request.UserId && d.Name == "Mobile Deck");

            if (deck == null)
            {
                deck = new FlashcardDeck
                {
                    Name = "Mobile Deck",
                    Description = "Flashcards created from the mobile app.",
                    CreatedAt = DateTime.Now,
                    UserId = request.UserId
                };

                _context.FlashcardDecks.Add(deck);
                await _context.SaveChangesAsync();
            }

            var flashcard = new Flashcard
            {
                Question = request.Question,
                Answer = request.Answer,
                FlashcardDeckId = deck.Id
            };

            _context.Flashcards.Add(flashcard);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                flashcard.Id,
                flashcard.Question,
                flashcard.Answer,
                flashcard.FlashcardDeckId,
                DeckName = deck.Name
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlashcard(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var flashcard = await _context.Flashcards
                .Include(f => f.FlashcardDeck)
                .FirstOrDefaultAsync(f =>
                    f.Id == id &&
                    f.FlashcardDeck != null &&
                    f.FlashcardDeck.UserId == userId);

            if (flashcard == null)
            {
                return NotFound();
            }

            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class MobileFlashcardRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;
    }
}