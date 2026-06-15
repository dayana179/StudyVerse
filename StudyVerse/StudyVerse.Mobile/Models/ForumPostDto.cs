namespace StudyVerse.Mobile.Models
{
    public class ForumPostDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string? Category { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? AttachmentPath { get; set; }

        public string? AttachmentFileName { get; set; }

        public List<ForumAttachmentDto> Attachments { get; set; } = new();

        public List<ForumReplyDto> Replies { get; set; } = new();
    }

    public class ForumAttachmentDto
    {
        public int AttachmentId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public long FileSize { get; set; }
        public string FullFileUrl { get; set; } = string.Empty;
    }

    public class ForumReplyDto
    {
        public int Id { get; set; }

        public string Content { get; set; } = string.Empty;

        public string? UserName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}