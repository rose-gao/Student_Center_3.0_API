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


        // Post: api/CourseEnrollmentService/AddCourse/{userNum}/{courseNum}
        [HttpPost("AddCourse/{userNum}/{courseNum}")]
        public async Task<ActionResult<string>> AddCourse(int userNum, int courseNum)
        {
            // Call the AddCourse method from CourseEnrollmentService
            string result = await _courseEnrollmentService.AddCourse(userNum, courseNum);

            // Return the result as a response
            if (result == "OK")
            {
                return Ok("Course successfully added."); // Success response with a message
            }

            // Return error message from the service
            return BadRequest($"Failed to add course: {result}");
        }

    }

}
