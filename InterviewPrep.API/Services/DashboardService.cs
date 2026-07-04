using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(Guid userId);
    }

    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardAsync(Guid userId)
        {
            var problems = await _context.Problems
                .Where(p => p.UserId == userId)
                .ToListAsync();

            var totalSolved = problems.Count;
            var easySolved = problems.Count(p => p.Difficulty == "Easy");
            var mediumSolved = problems.Count(p => p.Difficulty == "Medium");
            var hardSolved = problems.Count(p => p.Difficulty == "Hard");

            var topicBreakdown = problems
                .GroupBy(p => p.Topic)
                .Select(g => new TopicCountDto
                {
                    Topic = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count)
                .ToList();

            var platformBreakdown = problems
                .GroupBy(p => p.Platform)
                .Select(g => new PlatformCountDto
                {
                    Platform = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(p => p.Count)
                .ToList();

            var twelveMonthsAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12));

            var monthlyProgress = problems
                .Where(p => p.SolvedDate >= twelveMonthsAgo)
                .GroupBy(p => new { p.SolvedDate.Year, p.SolvedDate.Month })
                .Select(g => new MonthlyCountDto
                {
                    Year = g.Key.Year,
                    MonthNumber = g.Key.Month,
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Count = g.Count()
                })
                .OrderBy(m => m.Year).ThenBy(m => m.MonthNumber)
                .ToList();

            var streak = await _context.Streaks
                .FirstOrDefaultAsync(s => s.UserId == userId);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var streakDto = new StreakDto
            {
                CurrentStreak = streak?.CurrentStreak ?? 0,
                LongestStreak = streak?.LongestStreak ?? 0,
                LastSolvedDate = streak?.LastSolvedDate ?? today,
                SolvedToday = streak?.LastSolvedDate == today
            };

            var goals = await _context.Goals
                .Where(g => g.UserId == userId && !g.IsCompleted)
                .Select(g => new GoalResponseDto
                {
                    GoalId = g.GoalId,
                    Description = g.Description,
                    TargetCount = g.TargetCount,
                    CurrentCount = g.CurrentCount,
                    ProgressPercentage = g.TargetCount == 0 ? 0
                        : Math.Round((double)g.CurrentCount / g.TargetCount * 100, 1),
                    IsCompleted = g.IsCompleted,
                    Deadline = g.Deadline
                })
                .ToListAsync();

            return new DashboardDto
            {
                TotalSolved = totalSolved,
                EasySolved = easySolved,
                MediumSolved = mediumSolved,
                HardSolved = hardSolved,
                TopicBreakdown = topicBreakdown,
                PlatformBreakdown = platformBreakdown,
                MonthlyProgress = monthlyProgress,
                Streak = streakDto,
                ActiveGoals = goals
            };
        }
    }
}
