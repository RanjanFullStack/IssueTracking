using Microsoft.EntityFrameworkCore;

namespace IssueTracking.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One-to-Many relationship between Project and Issue
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Issues)
                .WithOne(i => i.Project)
                .HasForeignKey(i => i.ProjectId);

            // Many-to-Many relationship between Issue and Tag
            modelBuilder.Entity<Issue>()
                .HasMany(i => i.Tags)
                .WithMany(t => t.Issues)
                .UsingEntity(j => j.ToTable("IssueTags"));
        }
    }
}
