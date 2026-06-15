namespace StudyVerse.Mobile.Models
{
    public class ChatMessageDto
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public DateTime SentAt { get; set; }

        public int? ForumPostId { get; set; }

        public string? ForumPostTitle { get; set; }

        public bool IsOwnMessage => UserId == MobileUserSession.UserId;

        public bool HasForumPost => ForumPostId.HasValue;

        public string DisplayTime => SentAt.ToString("dd MMM yyyy, hh:mm tt");
    }
}