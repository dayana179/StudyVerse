using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyVerse.Models
{
    public class ForumPost
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Post Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}