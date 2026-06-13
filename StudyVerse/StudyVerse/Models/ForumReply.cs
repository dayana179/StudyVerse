using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyVerse.Models
{
    public class ForumReply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ForumPostId { get; set; }

        [ForeignKey("ForumPostId")]
        public ForumPost? ForumPost { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}