using Microsoft.EntityFrameworkCore;
using StudentDiary.Infrastructure.Models;

namespace StudentDiary.Infrastructure.Data
{
    public class StudentDiaryContext : DbContext
    {
        public StudentDiaryContext(DbContextOptions<StudentDiaryContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<DiaryEntry> DiaryEntries { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.DateCreated).HasDefaultValueSql("datetime('now')");
            });
            
            // DiaryEntry configuration
            modelBuilder.Entity<DiaryEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");
                entity.Property(e => e.LastModifiedDate).HasDefaultValueSql("datetime('now')");
                
                // Configure relationship
                entity.HasOne(d => d.User)
                      .WithMany(u => u.DiaryEntries)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}