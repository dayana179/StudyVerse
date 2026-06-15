namespace StudyVerse.Mobile.Models
{
    public class MobileFlashcard
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;
    }
}