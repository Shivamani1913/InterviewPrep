using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using InterviewPrep.API.Entities;
using InterviewPrep.API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (emailExists)
                throw new InvalidOperationException("An account with this email already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = passwordHash,
                Role = "User"
            };

            _context.Users.Add(user);

            var streak = new Streak
            {
                UserId = user.UserId,
                CurrentStreak = 0,
                LongestStreak = 0,
                LastSolvedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            _context.Streaks.Add(streak);

            await _context.SaveChangesAsync();

            return BuildAuthResponse(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return BuildAuthResponse(user);
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            return MapToUserDto(user);
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var token = _jwtHelper.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = MapToUserDto(user)
            };
        }

        private static UserDto MapToUserDto(User user) => new()
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
