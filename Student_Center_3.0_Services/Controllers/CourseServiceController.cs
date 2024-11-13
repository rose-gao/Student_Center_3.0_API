using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
