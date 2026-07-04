using InterviewPrep.API.Helpers;
using InterviewPrep.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly JwtHelper _jwtHelper;

        public DashboardController(IDashboardService dashboardService, JwtHelper jwtHelper)
        {
            _dashboardService = dashboardService;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = _jwtHelper.GetUserIdFromToken(User);
            var dashboard = await _dashboardService.GetDashboardAsync(userId);
            return Ok(dashboard);
        }
    }
}
