namespace StudyVerse.Mobile.Models
{
    public class MobileTaskItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}