namespace InterviewPrep.API.Entities
{
    public class Goal
    {
        public Guid GoalId { get; set; } = Guid.NewGuid();
        public string Description { get; set; } = string.Empty;
        public int TargetCount { get; set; }
        public int CurrentCount { get; set; } = 0;
        public DateTime? Deadline { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double ProgressPercentage => TargetCount == 0 ? 0 : Math.Round((double)CurrentCount / TargetCount * 100, 1);
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
