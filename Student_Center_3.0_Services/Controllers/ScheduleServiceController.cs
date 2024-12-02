using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Center_3._0_Services.Services;

namespace Student_Center_3._0_Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleServiceController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleServiceController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("schedule")]
        public async Task<IActionResult> GetSchedule(int userNum)
        {
            if (userNum < 0)
            {
                return BadRequest(new { Message = "Invalid student number." });

            }

            try
            {
                var schedule = await _scheduleService.GetSchedule(userNum);
                return Ok(new { Schedule = schedule });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the schedule.", Error = ex.Message });
            }


        }
    }
}
