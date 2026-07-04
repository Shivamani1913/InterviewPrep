namespace InterviewPrep.API.Entities
{
    public class User
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual ICollection<Problem> Problems { get; set; } = new List<Problem>();
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<Contest> Contests { get; set; } = new List<Contest>();
        public virtual ICollection<Goal> Goals { get; set; } = new List<Goal>();
        public virtual Streak? Streak { get; set; }
    }
}
