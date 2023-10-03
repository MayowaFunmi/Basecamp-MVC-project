using BasecampMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BasecampMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<MessageThread> MessageThreads { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.MessageThreads)
                .WithOne(mt => mt.Project);
                //.HasForeignKey<MessageThread>(mt => mt.ProjectId);

            modelBuilder.Entity<MessageThread>()
                .HasMany(mt => mt.Messages)
                .WithOne(m => m.Thread)
                .HasForeignKey(m => m.MessageThreadId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Thread)
                .WithMany(mt => mt.Messages)
                .HasForeignKey(m => m.MessageThreadId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

// We configure the relationship between Project and MessageThread using .HasOne and .WithOne to specify a one-to-one relationship.
// We configure the relationship between MessageThread and Message using .HasMany and .WithOne to specify a one-to-many relationship.
// We configure the relationship between Message and User to associate messages with users.