using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Center_3._0_Services.Services;

namespace Student_Center_3._0_Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseEnrollmentServiceController : ControllerBase
    {
        private readonly CourseEnrollmentService _courseEnrollmentService;

        // Constructor that accepts the CourseService
        public CourseEnrollmentServiceController(CourseEnrollmentService courseEnrollmentService)
        {
            _courseEnrollmentService = courseEnrollmentService;
        }

        // GET: api/CourseEnrollmentService/VerifyEnrollmentRequirements/{userNum}/{requestedCourse}
        [HttpGet("VerifyEnrollmentRequirements/{userNum}/{requestedCourse}")]
        public async Task<ActionResult<bool>> VerifyEnrollmentRequirements(int userNum, string requestedCourse)
        {
            // Call the CheckPrerequisite method from CourseService
            bool result = await _courseEnrollmentService.VerifyEnrollmentRequirements(userNum, requestedCourse);

            // Return the result as a response
            if (result)
            {
                return Ok(true); // If prerequisites are fulfilled, return true
            }
            return BadRequest("Prerequisite not fulfilled or no course history found.");
        }
    }

}
