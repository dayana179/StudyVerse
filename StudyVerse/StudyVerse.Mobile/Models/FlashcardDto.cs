namespace StudyVerse.Mobile.Models
{
    public class FlashcardDeckDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CardCount { get; set; }

        public string CardCountText => $"{CardCount} cards";
    }

    public class FlashcardDto
    {
        public int Id { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int FlashcardDeckId { get; set; }
    }
}