using InterviewPrep.API.DTOs;
using InterviewPrep.API.Helpers;
using InterviewPrep.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly JwtHelper _jwtHelper;

        public GoalsController(IGoalService goalService, JwtHelper jwtHelper)
        {
            _goalService = goalService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetGoals()
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var goals = await _goalService.GetAllAsync(userId);
            return Ok(goals);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalDto dto)
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var goal = await _goalService.CreateAsync(userId, dto);
            return Ok(goal);
        }

        [HttpPut("{id:guid}/progress")]
        public async Task<IActionResult> UpdateProgress(Guid id, [FromBody] UpdateProgressDto dto)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                var goal = await _goalService.UpdateProgressAsync(id, userId, dto.CurrentCount);
                return Ok(goal);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Goal not found." });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGoal(Guid id)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                await _goalService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Goal not found." });
            }
        }
    }
}
