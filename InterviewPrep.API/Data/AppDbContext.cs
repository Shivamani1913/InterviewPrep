using InterviewPrep.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Streak> Streaks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(200).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).HasMaxLength(50).HasDefaultValue("User");
            });

            modelBuilder.Entity<Problem>(entity =>
            {
                entity.HasKey(p => p.ProblemId);
                entity.Property(p => p.Title).HasMaxLength(300).IsRequired();
                entity.Property(p => p.Platform).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Difficulty).HasMaxLength(20).IsRequired();
                entity.Property(p => p.Topic).HasMaxLength(100).IsRequired();
                entity.Property(p => p.Notes).HasMaxLength(2000);
                entity.Property(p => p.ProblemUrl).HasMaxLength(500);
                entity.HasOne(p => p.User)
                      .WithMany(u => u.Problems)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(n => n.NoteId);
                entity.Property(n => n.Title).HasMaxLength(300).IsRequired();
                entity.Property(n => n.Topic).HasMaxLength(100).IsRequired();
                entity.Property(n => n.Content).IsRequired();
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notes)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Contest>(entity =>
            {
                entity.HasKey(c => c.ContestId);
                entity.Property(c => c.ContestName).HasMaxLength(300).IsRequired();
                entity.Property(c => c.Platform).HasMaxLength(100).IsRequired();
                entity.Ignore(c => c.RatingChange);
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Contests)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasKey(g => g.GoalId);
                entity.Property(g => g.Description).HasMaxLength(500).IsRequired();
                entity.Ignore(g => g.ProgressPercentage);
                entity.HasOne(g => g.User)
                      .WithMany(u => u.Goals)
                      .HasForeignKey(g => g.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Streak>(entity =>
            {
                entity.HasKey(s => s.StreakId);
                entity.HasOne(s => s.User)
                      .WithOne(u => u.Streak)
                      .HasForeignKey<Streak>(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
