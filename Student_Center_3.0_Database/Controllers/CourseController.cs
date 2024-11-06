using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                courseName = courseDTO.courseName,
                courseSuffix = courseDTO.courseSuffix,
                courseDesc = courseDTO.courseDesc,
                extraInformation = courseDTO.extraInformation,
                prerequisites = courseDTO.prerequisites,
                antirequisites = courseDTO.antirequisites,
                courseSemester = courseDTO.courseSemester,
                courseDay = courseDTO.courseDay,
                courseTime = courseDTO.courseTime,
                instructor = courseDTO.instructor,
                room = courseDTO.room,  
                numEnrolled = courseDTO.numEnrolled,
                totalSeats = courseDTO.totalSeats,
                isLab = courseDTO.isLab
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
                    courseName = courseDTO.courseName,
                    courseSuffix = courseDTO.courseSuffix,
                    courseDesc = courseDTO.courseDesc,
                    extraInformation = courseDTO.extraInformation,
                    prerequisites = courseDTO.prerequisites,
                    antirequisites = courseDTO.antirequisites,
                    courseSemester = courseDTO.courseSemester,
                    courseDay = courseDTO.courseDay,
                    courseTime = courseDTO.courseTime,
                    instructor = courseDTO.instructor,
                    room = courseDTO.room,
                    numEnrolled = courseDTO.numEnrolled,
                    totalSeats = courseDTO.totalSeats,
                    isLab = courseDTO.isLab
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

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.courseNum == id);
        }
    }
}
