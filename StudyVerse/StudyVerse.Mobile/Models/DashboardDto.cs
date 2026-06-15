namespace StudyVerse.Mobile.Models
{
    public class DashboardDto
    {
        public int TotalTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int PendingTasks { get; set; }

        public int Progress { get; set; }

        public List<DashboardTaskDto> TodayTasks { get; set; } = new();

        public List<DashboardTaskDto> UpcomingTasks { get; set; } = new();

        public List<DashboardDeckDto> RecentDecks { get; set; } = new();

        public List<DashboardPostDto> RecentPosts { get; set; } = new();
    }

    public class DashboardTaskDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Priority { get; set; }

        public DateTime DueDate { get; set; }
    }

    public class DashboardDeckDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }

    public class DashboardPostDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
    }
}