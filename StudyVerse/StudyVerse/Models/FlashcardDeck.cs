using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyVerse.Models
{
    public class FlashcardDeck
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Deck Name")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public ICollection<Flashcard>? Flashcards { get; set; }
    }
}