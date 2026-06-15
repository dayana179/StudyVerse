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

        [HttpGet("decks")]
        public async Task<IActionResult> GetDecks([FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var decks = await _context.FlashcardDecks
                .Where(d => d.UserId == userId)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Description,
                    d.CreatedAt,
                    CardCount = d.Flashcards.Count
                })
                .ToListAsync();

            return Ok(decks);
        }

        [HttpPost("decks")]
        public async Task<IActionResult> CreateDeck([FromBody] MobileDeckRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("User ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Deck name is required.");
            }

            var deck = new FlashcardDeck
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                CreatedAt = DateTime.Now,
                UserId = request.UserId
            };

            _context.FlashcardDecks.Add(deck);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                deck.Id,
                deck.Name,
                deck.Description,
                deck.CreatedAt,
                CardCount = 0
            });
        }

        [HttpGet("decks/{deckId}/cards")]
        public async Task<IActionResult> GetCards(int deckId, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var deck = await _context.FlashcardDecks
                .FirstOrDefaultAsync(d => d.Id == deckId && d.UserId == userId);

            if (deck == null)
            {
                return NotFound("Deck not found.");
            }

            var cards = await _context.Flashcards
                .Where(c => c.FlashcardDeckId == deckId)
                .OrderBy(c => c.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Question,
                    c.Answer,
                    c.FlashcardDeckId
                })
                .ToListAsync();

            return Ok(cards);
        }

        [HttpPost("decks/{deckId}/cards")]
        public async Task<IActionResult> CreateCard(int deckId, [FromBody] MobileCardRequest request)
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
                .FirstOrDefaultAsync(d => d.Id == deckId && d.UserId == request.UserId);

            if (deck == null)
            {
                return NotFound("Deck not found.");
            }

            var card = new Flashcard
            {
                Question = request.Question.Trim(),
                Answer = request.Answer.Trim(),
                FlashcardDeckId = deckId
            };

            _context.Flashcards.Add(card);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                card.Id,
                card.Question,
                card.Answer,
                card.FlashcardDeckId
            });
        }

        [HttpDelete("cards/{cardId}")]
        public async Task<IActionResult> DeleteCard(int cardId, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var card = await _context.Flashcards
                .Include(c => c.FlashcardDeck)
                .FirstOrDefaultAsync(c =>
                    c.Id == cardId &&
                    c.FlashcardDeck != null &&
                    c.FlashcardDeck.UserId == userId);

            if (card == null)
            {
                return NotFound();
            }

            _context.Flashcards.Remove(card);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("decks/{deckId}")]
        public async Task<IActionResult> DeleteDeck(int deckId, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var deck = await _context.FlashcardDecks
                .Include(d => d.Flashcards)
                .FirstOrDefaultAsync(d => d.Id == deckId && d.UserId == userId);

            if (deck == null)
            {
                return NotFound();
            }

            _context.Flashcards.RemoveRange(deck.Flashcards);
            _context.FlashcardDecks.Remove(deck);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class MobileDeckRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class MobileCardRequest
    {
        public string UserId { get; set; } = string.Empty;

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;
    }
}