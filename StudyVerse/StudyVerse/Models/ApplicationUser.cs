using Microsoft.AspNetCore.Identity;

namespace StudyVerse.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}