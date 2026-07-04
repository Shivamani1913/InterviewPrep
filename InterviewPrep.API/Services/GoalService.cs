using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using InterviewPrep.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Services
{
    public interface IGoalService
    {
        Task<List<GoalResponseDto>> GetAllAsync(Guid userId);
        Task<GoalResponseDto> CreateAsync(Guid userId, CreateGoalDto dto);
        Task<GoalResponseDto> UpdateProgressAsync(Guid goalId, Guid userId, int currentCount);
        Task DeleteAsync(Guid goalId, Guid userId);
    }

    public class GoalService : IGoalService
    {
        private readonly AppDbContext _context;

        public GoalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GoalResponseDto>> GetAllAsync(Guid userId)
        {
            return await _context.Goals
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.IsCompleted)
                .ThenByDescending(g => g.CreatedAt)
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
        }

        public async Task<GoalResponseDto> CreateAsync(Guid userId, CreateGoalDto dto)
        {
            var goal = new Goal
            {
                UserId = userId,
                Description = dto.Description.Trim(),
                TargetCount = dto.TargetCount,
                CurrentCount = 0,
                Deadline = dto.Deadline,
                IsCompleted = false
            };

            _context.Goals.Add(goal);
            await _context.SaveChangesAsync();

            return new GoalResponseDto
            {
                GoalId = goal.GoalId,
                Description = goal.Description,
                TargetCount = goal.TargetCount,
                CurrentCount = goal.CurrentCount,
                ProgressPercentage = 0,
                IsCompleted = goal.IsCompleted,
                Deadline = goal.Deadline
            };
        }

        public async Task<GoalResponseDto> UpdateProgressAsync(Guid goalId, Guid userId, int currentCount)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);

            if (goal == null)
                throw new KeyNotFoundException("Goal not found.");

            goal.CurrentCount = currentCount;
            goal.IsCompleted = currentCount >= goal.TargetCount;

            await _context.SaveChangesAsync();

            return new GoalResponseDto
            {
                GoalId = goal.GoalId,
                Description = goal.Description,
                TargetCount = goal.TargetCount,
                CurrentCount = goal.CurrentCount,
                ProgressPercentage = goal.TargetCount == 0 ? 0
                    : Math.Round((double)goal.CurrentCount / goal.TargetCount * 100, 1),
                IsCompleted = goal.IsCompleted,
                Deadline = goal.Deadline
            };
        }

        public async Task DeleteAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.GoalId == goalId && g.UserId == userId);

            if (goal == null)
                throw new KeyNotFoundException("Goal not found.");

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
        }
    }
}
