using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyVerse.Models
{
    public class ForumAttachment
    {
        [Key]
        public int AttachmentId { get; set; }

        [Required]
        public int ForumPostId { get; set; }

        [ForeignKey("ForumPostId")]
        public ForumPost? ForumPost { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public string? ContentType { get; set; }

        public long FileSize { get; set; }
    }
}