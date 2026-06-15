namespace StudyVerse.Mobile.Models
{
    public class FlashcardDto
    {
        public int Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int FlashcardDeckId { get; set; }

        public string? DeckName { get; set; }
    }
}