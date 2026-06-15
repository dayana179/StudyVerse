namespace StudyVerse.Mobile.Models
{
    public class TaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public string? Priority { get; set; }

        public bool IsCompleted { get; set; }

        public string StatusText => IsCompleted ? "Completed" : "Pending";
    }
}