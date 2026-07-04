namespace InterviewPrep.API.Entities
{
    public class Streak
    {
        public Guid StreakId { get; set; } = Guid.NewGuid();
        public int CurrentStreak { get; set; } = 0;
        public int LongestStreak { get; set; } = 0;
        public DateOnly LastSolvedDate { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
