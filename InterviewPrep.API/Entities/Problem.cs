namespace InterviewPrep.API.Entities
{
    public class Problem
    {
        public Guid ProblemId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public DateOnly SolvedDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public string? Notes { get; set; }
        public string? ProblemUrl { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
