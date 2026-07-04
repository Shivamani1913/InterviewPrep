using InterviewPrep.API.Data;
using InterviewPrep.API.DTOs;
using InterviewPrep.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InterviewPrep.API.Controllers
{
    public class UpdateProfileDto
    {
        [MaxLength(100)] public string? Name { get; set; }
        [EmailAddress][MaxLength(200)] public string? Email { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public ProfileController(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            if (dto.Email != null)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == dto.Email.ToLower() && u.UserId != userId);
                if (emailExists)
                    return BadRequest(new { message = "Email already in use." });

                user.Email = dto.Email.ToLower().Trim();
            }

            if (dto.Name != null)
                user.Name = dto.Name.Trim();

            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }
    }
}
