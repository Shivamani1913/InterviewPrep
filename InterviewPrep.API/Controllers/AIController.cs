using InterviewPrep.API.DTOs;
using InterviewPrep.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("explain")]
        public async Task<IActionResult> ExplainTopic([FromBody] ExplainTopicDto dto)
        {
            try
            {
                var result = await _aiService.ExplainTopicAsync(dto.Topic, dto.Difficulty);
                return Ok(new AIResponseDto
                {
                    Result = result,
                    Topic = dto.Topic
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "AI service error: " + ex.Message });
            }
        }

        [HttpPost("mock-interview")]
        public async Task<IActionResult> GenerateMockInterview([FromBody] MockInterviewDto dto)
        {
            try
            {
                var result = await _aiService.GenerateMockInterviewAsync(dto.Topic, dto.Difficulty);
                return Ok(new AIResponseDto
                {
                    Result = result,
                    Topic = dto.Topic
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "AI service error: " + ex.Message });
            }
        }

        [HttpPost("analyze-resume")]
        public async Task<IActionResult> AnalyzeResume([FromBody] ResumeAnalyzeDto dto)
        {
            try
            {
                var result = await _aiService.AnalyzeResumeAsync(dto.ResumeText);
                return Ok(new AIResponseDto
                {
                    Result = result,
                    Topic = "Resume Analysis"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "AI service error: " + ex.Message });
            }
        }
    }
}
