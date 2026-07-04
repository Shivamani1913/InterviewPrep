using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using InterviewPrep.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Services
{
    public interface IContestService
    {
        Task<List<ContestResponseDto>> GetAllAsync(Guid userId);
        Task<ContestResponseDto> CreateAsync(Guid userId, CreateContestDto dto);
        Task DeleteAsync(Guid contestId, Guid userId);
    }

    public class ContestService : IContestService
    {
        private readonly AppDbContext _context;

        public ContestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContestResponseDto>> GetAllAsync(Guid userId)
        {
            return await _context.Contests
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.ContestDate)
                .Select(c => new ContestResponseDto
                {
                    ContestId = c.ContestId,
                    ContestName = c.ContestName,
                    Platform = c.Platform,
                    Rank = c.Rank,
                    RatingBefore = c.RatingBefore,
                    RatingAfter = c.RatingAfter,
                    RatingChange = c.RatingAfter - c.RatingBefore,
                    ContestDate = c.ContestDate
                })
                .ToListAsync();
        }

        public async Task<ContestResponseDto> CreateAsync(Guid userId, CreateContestDto dto)
        {
            var contest = new Contest
            {
                UserId = userId,
                ContestName = dto.ContestName.Trim(),
                Platform = dto.Platform.Trim(),
                Rank = dto.Rank,
                RatingBefore = dto.RatingBefore,
                RatingAfter = dto.RatingAfter,
                ContestDate = dto.ContestDate
            };

            _context.Contests.Add(contest);
            await _context.SaveChangesAsync();

            return new ContestResponseDto
            {
                ContestId = contest.ContestId,
                ContestName = contest.ContestName,
                Platform = contest.Platform,
                Rank = contest.Rank,
                RatingBefore = contest.RatingBefore,
                RatingAfter = contest.RatingAfter,
                RatingChange = contest.RatingAfter - contest.RatingBefore,
                ContestDate = contest.ContestDate
            };
        }

        public async Task DeleteAsync(Guid contestId, Guid userId)
        {
            var contest = await _context.Contests
                .FirstOrDefaultAsync(c => c.ContestId == contestId && c.UserId == userId);

            if (contest == null)
                throw new KeyNotFoundException("Contest not found.");

            _context.Contests.Remove(contest);
            await _context.SaveChangesAsync();
        }
    }
}
