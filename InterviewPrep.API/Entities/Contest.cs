namespace InterviewPrep.API.Entities
{
    public class Contest
    {
        public Guid ContestId { get; set; } = Guid.NewGuid();
        public string ContestName { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int RatingBefore { get; set; }
        public int RatingAfter { get; set; }
        public int RatingChange => RatingAfter - RatingBefore;
        public DateOnly ContestDate { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
    