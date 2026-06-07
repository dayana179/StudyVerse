using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyVerse.Models
{
    public class Flashcard
    {
        public int Id { get; set; }

        [Required]
        public string Question { get; set; } = string.Empty;

        [Required]
        public string Answer { get; set; } = string.Empty;

        public int FlashcardDeckId { get; set; }

        [ForeignKey("FlashcardDeckId")]
        public FlashcardDeck? FlashcardDeck { get; set; }
    }
}