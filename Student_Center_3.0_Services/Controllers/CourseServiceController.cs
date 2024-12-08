using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Center_3._0_Services.DTOs;
using Student_Center_3._0_Services.Services;

namespace Student_Center_3._0_Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseServiceController : ControllerBase
    {
        private readonly CourseService _courseService;

        // Constructor that accepts the CourseService
        public CourseServiceController(CourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/Course/GetCoursesBySearch/search?query={searchString}
        [HttpGet("search")]
        public async Task<ActionResult<List<Dictionary<string, object>>>> GetCoursesBySearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query string cannot be empty.");
            }

            var courses = await _courseService.GetCoursesLabsBySearch(query);

            if (courses == null || !courses.Any())
            {
                return NotFound("No matching courses found.");
            }

            return Ok(courses);
        }

        // POST: api/CourseServicesController
        [HttpPost]
        public async Task<IActionResult> AddCourse([FromBody] CourseCreateDTO courseCreateDTO)
        {
            // For each user attribute, make some basic validation checks using the data annotaionts of UserDTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // try to add user, incl. more complex validation logic
                var result = await _courseService.AddCourse(courseCreateDTO);

                if (result == "OK")
                {
                    return Ok(new { Message = "Course successfully added." });
                }

                return BadRequest(new { Message = result });
            }
            catch (Exception ex)
            {
                // Log the exception (optional: using a logging library like Serilog or NLog)
                return StatusCode(500, new { Message = "An internal error occurred.", Details = ex.Message });
            }
        }

    }
}
