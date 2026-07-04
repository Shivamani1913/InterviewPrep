using System.ComponentModel.DataAnnotations;

namespace InterviewPrep.API.DTOs
{
    public class CreateNoteDto
    {
        [Required][MaxLength(300)] public string Title { get; set; } = string.Empty;
        [Required] public string Content { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Topic { get; set; } = string.Empty;
    }

    public class UpdateNoteDto
    {
        [MaxLength(300)] public string? Title { get; set; }
        public string? Content { get; set; }
        [MaxLength(100)] public string? Topic { get; set; }
    }

    public class NoteResponseDto
    {
        public Guid NoteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateContestDto
    {
        [Required][MaxLength(300)] public string ContestName { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Platform { get; set; } = string.Empty;
        [Range(1, int.MaxValue)] public int Rank { get; set; }
        public int RatingBefore { get; set; }
        public int RatingAfter { get; set; }
        public DateOnly ContestDate { get; set; }
    }

    public class ContestResponseDto
    {
        public Guid ContestId { get; set; }
        public string ContestName { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int RatingBefore { get; set; }
        public int RatingAfter { get; set; }
        public int RatingChange { get; set; }
        public DateOnly ContestDate { get; set; }
    }

    public class CreateGoalDto
    {
        [Required][MaxLength(500)] public string Description { get; set; } = string.Empty;
        [Range(1, 10000)] public int TargetCount { get; set; }
        public DateTime? Deadline { get; set; }
    }

    public class UpdateProgressDto
    {
        [Range(0, 10000)] public int CurrentCount { get; set; }
    }

    public class GoalResponseDto
    {
        public Guid GoalId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int TargetCount { get; set; }
        public int CurrentCount { get; set; }
        public double ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? Deadline { get; set; }
    }

    public class StreakDto
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateOnly LastSolvedDate { get; set; }
        public bool SolvedToday { get; set; }
    }

    public class DashboardDto
    {
        public int TotalSolved { get; set; }
        public int EasySolved { get; set; }
        public int MediumSolved { get; set; }
        public int HardSolved { get; set; }
        public List<TopicCountDto> TopicBreakdown { get; set; } = new();
        public List<PlatformCountDto> PlatformBreakdown { get; set; } = new();
        public List<MonthlyCountDto> MonthlyProgress { get; set; } = new();
        public StreakDto Streak { get; set; } = new();
        public List<GoalResponseDto> ActiveGoals { get; set; } = new();
    }

    public class TopicCountDto
    {
        public string Topic { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class PlatformCountDto
    {
        public string Platform { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MonthlyCountDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
        public int Year { get; set; }
        public int MonthNumber { get; set; }
    }
}
