using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudyVerse.Models;

namespace StudyVerse.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<FlashcardDeck> FlashcardDecks { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }
    }
}