using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using InterviewPrep.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Services
{
    public interface IProblemService
    {
        Task<PagedResponseDto<ProblemResponseDto>> GetProblemsAsync(Guid userId, ProblemFilterDto filter);
        Task<ProblemResponseDto> GetByIdAsync(Guid problemId, Guid userId);
        Task<ProblemResponseDto> CreateAsync(Guid userId, CreateProblemDto dto);
        Task<ProblemResponseDto> UpdateAsync(Guid problemId, Guid userId, UpdateProblemDto dto);
        Task DeleteAsync(Guid problemId, Guid userId);
    }

    public class ProblemService : IProblemService
    {
        private readonly AppDbContext _context;

        public ProblemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponseDto<ProblemResponseDto>> GetProblemsAsync(Guid userId, ProblemFilterDto filter)
        {
            var query = _context.Problems
                .Where(p => p.UserId == userId);

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(p => p.Title.Contains(filter.Search) || p.Topic.Contains(filter.Search));

            if (!string.IsNullOrWhiteSpace(filter.Difficulty))
                query = query.Where(p => p.Difficulty == filter.Difficulty);

            if (!string.IsNullOrWhiteSpace(filter.Topic))
                query = query.Where(p => p.Topic == filter.Topic);

            if (!string.IsNullOrWhiteSpace(filter.Platform))
                query = query.Where(p => p.Platform == filter.Platform);

            var totalCount = await query.CountAsync();

            var problems = await query
                .OrderByDescending(p => p.SolvedDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResponseDto<ProblemResponseDto>
            {
                Items = problems.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<ProblemResponseDto> GetByIdAsync(Guid problemId, Guid userId)
        {
            var problem = await _context.Problems
                .FirstOrDefaultAsync(p => p.ProblemId == problemId && p.UserId == userId);

            if (problem == null)
                throw new KeyNotFoundException("Problem not found.");

            return MapToDto(problem);
        }

        public async Task<ProblemResponseDto> CreateAsync(Guid userId, CreateProblemDto dto)
        {
            var problem = new Problem
            {
                UserId = userId,
                Title = dto.Title.Trim(),
                Platform = dto.Platform.Trim(),
                Difficulty = dto.Difficulty,
                Topic = dto.Topic.Trim(),
                SolvedDate = dto.SolvedDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
                Notes = dto.Notes?.Trim(),
                ProblemUrl = dto.ProblemUrl?.Trim()
            };

            _context.Problems.Add(problem);
            await UpdateStreakAsync(userId, problem.SolvedDate);
            await _context.SaveChangesAsync();

            return MapToDto(problem);
        }

        public async Task<ProblemResponseDto> UpdateAsync(Guid problemId, Guid userId, UpdateProblemDto dto)
        {
            var problem = await _context.Problems
                .FirstOrDefaultAsync(p => p.ProblemId == problemId && p.UserId == userId);

            if (problem == null)
                throw new KeyNotFoundException("Problem not found.");

            if (dto.Title != null) problem.Title = dto.Title.Trim();
            if (dto.Platform != null) problem.Platform = dto.Platform.Trim();
            if (dto.Difficulty != null) problem.Difficulty = dto.Difficulty;
            if (dto.Topic != null) problem.Topic = dto.Topic.Trim();
            if (dto.SolvedDate.HasValue) problem.SolvedDate = dto.SolvedDate.Value;
            if (dto.Notes != null) problem.Notes = dto.Notes.Trim();
            if (dto.ProblemUrl != null) problem.ProblemUrl = dto.ProblemUrl.Trim();

            await _context.SaveChangesAsync();

            return MapToDto(problem);
        }

        public async Task DeleteAsync(Guid problemId, Guid userId)
        {
            var problem = await _context.Problems
                .FirstOrDefaultAsync(p => p.ProblemId == problemId && p.UserId == userId);

            if (problem == null)
                throw new KeyNotFoundException("Problem not found.");

            _context.Problems.Remove(problem);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateStreakAsync(Guid userId, DateOnly solvedDate)
        {
            var streak = await _context.Streaks
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (streak == null) return;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (streak.LastSolvedDate == solvedDate)
                return;

            if (streak.LastSolvedDate == today.AddDays(-1))
                streak.CurrentStreak++;
            else
                streak.CurrentStreak = 1;

            streak.LastSolvedDate = solvedDate;

            if (streak.CurrentStreak > streak.LongestStreak)
                streak.LongestStreak = streak.CurrentStreak;
        }

        private static ProblemResponseDto MapToDto(Problem p) => new()
        {
            ProblemId = p.ProblemId,
            Title = p.Title,
            Platform = p.Platform,
            Difficulty = p.Difficulty,
            Topic = p.Topic,
            SolvedDate = p.SolvedDate,
            Notes = p.Notes,
            ProblemUrl = p.ProblemUrl,
            UserId = p.UserId
        };
    }
}
