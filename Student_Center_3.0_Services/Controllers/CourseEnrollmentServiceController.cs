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
        [HttpPost("AddCourses/{userNum}")]
        public async Task<ActionResult<string>> AddCourses(int userNum, [FromBody] List<int> courseNums)
        {
            if (courseNums == null || !courseNums.Any())
            {
                return BadRequest("Course list cannot be null or empty.");
            }

            // Call the AddCourse method from CourseEnrollmentService
            string result = await _courseEnrollmentService.AddCourse(userNum, courseNums);

            // Return the result as a response
            if (result == "OK")
            {
                return Ok("Courses successfully added."); // Success response with a message
            }

            // Return error message from the service
            return BadRequest($"Failed to add courses: {result}");
        }


        // Post: api/CourseEnrollmentService/DropCourse/{userNum}/{courseNum}
        [HttpPost("DropCourse/{userNum}/{courseNum}")]
        public async Task<ActionResult<string>> DropCourse(int userNum, int courseNum)
        {
            if (userNum <= 0 || courseNum <= 0)
            {
                return BadRequest("UserNum and CourseNum must be greater than zero.");
            }

            // Call the AddCourse method from CourseEnrollmentService
            string result = await _courseEnrollmentService.DropCourse(userNum, courseNum);

            // Return the result as a response
            if (result == "OK")
            {
                return Ok("Course successfully dropped."); // Success response with a message
            }

            // Return error message from the service
            return BadRequest($"Failed to drop course: {result}");
        }

        // Post: api/CourseEnrollmentService/SwapCourse/{userNum}/{dropCourseNum}/{addCourseNum}
        [HttpPost("SwapCourse/{userNum}/{dropCourseNum}")]
        public async Task<ActionResult<string>> SwapCourse(int userNum, int dropCourseNum, [FromBody] List<int> addCourseNums)
        {
            // Validate input
            if (userNum <= 0 || dropCourseNum <= 0 || addCourseNums == null || !addCourseNums.Any() || addCourseNums.Any(c => c <= 0))
            {
                return BadRequest("Invalid input. Ensure userNum, dropCourseNum, and addCourseNums are valid.");
            }

            if (addCourseNums.Contains(dropCourseNum))
                return BadRequest("Cannot swap the same course.");

            // Call the AddCourse method from CourseEnrollmentService
            string result = await _courseEnrollmentService.SwapCourse(userNum, dropCourseNum, addCourseNums);

            // Return the result as a response
            if (result == "OK")
            {
                return Ok("Courses successfully swapped."); // Success response with a message
            }

            // Return error message from the service
            return BadRequest($"Failed to swap courses: {result}");
        }

    }

}
