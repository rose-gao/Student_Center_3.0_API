using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Student_Center_3._0_Database.DTOs;
using Student_Center_3._0_Database.Models;
using Student_Center_3._0_Database.Utils;

namespace Student_Center_3._0_Database.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public CourseController(StudentCenterContext context)
        {
            _context = context;
        }


        // GET: api/Course
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        // GET: api/Course/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Course/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, CourseDTO courseDTO)
        {
            var course = new Course
            {
                courseNum = id,
                courseName = courseDTO.courseName.Trim(),
                courseSuffix = courseDTO.courseSuffix.Trim(),
                courseAlias = courseDTO.courseAlias.Trim(),
                courseDesc = courseDTO.courseDesc,
                extraInformation = courseDTO.extraInformation?.Trim() ?? string.Empty,
                prerequisites = courseDTO.prerequisites?.Trim() ?? string.Empty,
                antirequisites = courseDTO.antirequisites?.Trim() ?? string.Empty,
                courseWeight = courseDTO.courseWeight,
                startDate = courseDTO.startDate,
                endDate = courseDTO.endDate,
                instructor = courseDTO.instructor?.Trim() ?? string.Empty,
                room = courseDTO.room?.Trim() ?? string.Empty,
                numEnrolled = courseDTO.numEnrolled,
                totalSeats = courseDTO.totalSeats
            };

            if (id != course.courseNum)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Course
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Course>> PostCourse(CourseDTO courseDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var course = new Course
                {
                    courseName = courseDTO.courseName.Trim(),
                    courseSuffix = courseDTO.courseSuffix.Trim(),
                    courseAlias = courseDTO.courseAlias.Trim(),
                    courseDesc = courseDTO.courseDesc,
                    extraInformation = courseDTO.extraInformation?.Trim() ?? string.Empty,
                    prerequisites = courseDTO.prerequisites?.Trim() ?? string.Empty,
                    antirequisites = courseDTO.antirequisites?.Trim() ?? string.Empty,
                    courseWeight = courseDTO.courseWeight,
                    startDate = courseDTO.startDate,
                    endDate = courseDTO.endDate,
                    instructor = courseDTO.instructor?.Trim() ?? string.Empty,
                    room = courseDTO.room?.Trim() ?? string.Empty,
                    numEnrolled = courseDTO.numEnrolled,
                    totalSeats = courseDTO.totalSeats
                };

                try
                {
                    _context.Courses.Add(course);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return CreatedAtAction("GetCourse", new { id = course.courseNum }, course);
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.Message}");
                }

            }
        }

        // DELETE: api/Course/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        // GET: api/Course/search?query={searchString}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Course>>> SearchCourses(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query string cannot be empty.");
            }

            // Try to parse query as a course number
            //bool isNumeric = int.TryParse(query, out int courseNumSearch);

            // Convert query to lowercase for case-insensitive search
            query = query.ToLower();

            // Filter courses based on the query and group by courseName and courseNum to remove duplicates
            var filteredCourses = await _context.Courses
                .Where(course =>
                    // Case-insensitive search by converting courseName to lowercase, concatenate courseName and courseSuffix for searches like "1026B"
                    (course.courseName.ToLower() + course.courseSuffix.ToLower()).Contains(query))
                    // Check if courseNum matches when query is numeric
                    // || (isNumeric && course.courseNum == courseNumSearch))
                // Group by courseName and courseNum to ensure distinct results
                .GroupBy(course => new { course.courseName, course.courseNum })
                .Select(g => g.First())  // Select only one instance per group
                .ToListAsync();

            return Ok(filteredCourses);
        }
        

        [HttpPatch("{courseNum}/update-enrollment")]
        public async Task<IActionResult> UpdateCourseEnrollment(int courseNum, [FromBody] int updatedNumEnrolled)
        {
            // Find the course by its courseNum
            var course = await _context.Courses.FindAsync(courseNum);

            if (course == null)
            {
                return NotFound($"Course with courseNum {courseNum} not found.");
            }

            // Update the numEnrolled column
            course.numEnrolled = updatedNumEnrolled;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return Ok($"numEnrolled for course {courseNum} updated to {updatedNumEnrolled}.");
            }
            catch (Exception ex)
            {
                // Handle potential errors
                return StatusCode(500, $"An error occurred while updating the course: {ex.Message}");
            }
        }



        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.courseNum == id);
        }
    }
}
