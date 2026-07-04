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
    public class ProblemsController : ControllerBase
    {
        private readonly IProblemService _problemService;
        private readonly JwtHelper _jwtHelper;

        public ProblemsController(IProblemService problemService, JwtHelper jwtHelper)
        {
            _problemService = problemService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProblems([FromQuery] ProblemFilterDto filter)
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var result = await _problemService.GetProblemsAsync(userId, filter);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProblem(Guid id)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                var problem = await _problemService.GetByIdAsync(id, userId);
                return Ok(problem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Problem not found." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProblem([FromBody] CreateProblemDto dto)
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var problem = await _problemService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetProblem), new { id = problem.ProblemId }, problem);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProblem(Guid id, [FromBody] UpdateProblemDto dto)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                var problem = await _problemService.UpdateAsync(id, userId, dto);
                return Ok(problem);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Problem not found." });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProblem(Guid id)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                await _problemService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Problem not found." });
            }
        }
    }
}
