using InterviewPrep.API.DTOs;
using InterviewPrep.API.Helpers;
using InterviewPrep.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableCors("AllowAll")]
    public class ContestsController : ControllerBase
    {
        private readonly IContestService _contestService;
        private readonly JwtHelper _jwtHelper;

        public ContestsController(IContestService contestService, JwtHelper jwtHelper)
        {
            _contestService = contestService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetContests()
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var contests = await _contestService.GetAllAsync(userId);
            return Ok(contests);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContest([FromBody] CreateContestDto dto)
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var contest = await _contestService.CreateAsync(userId, dto);
            return Ok(contest);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteContest(Guid id)
        {
            try
            {
                var userId = _jwtHelper.GetUserIdFromToken(User);
                await _contestService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contest not found." });
            }
        }
    }
}
