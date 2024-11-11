using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Center_3._0_Services.Services;

namespace Student_Center_3._0_Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;

        // Constructor that accepts the CourseService
        public CourseController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/Course/checkPrerequisite/{userNum}/{requestedCourse}
        [HttpGet("checkPrerequisite/{userNum}/{requestedCourse}")]
        public async Task<ActionResult<bool>> CheckPrerequisite(int userNum, string requestedCourse)
        {
            // Call the CheckPrerequisite method from CourseService
            bool result = await _courseService.CheckPrerequisite(userNum, requestedCourse);

            // Return the result as a response
            if (result)
            {
                return Ok(true); // If prerequisites are fulfilled, return true
            }
            else
            {
                return BadRequest("Prerequisite not fulfilled or no course history found.");
            }
        }
    }

}
