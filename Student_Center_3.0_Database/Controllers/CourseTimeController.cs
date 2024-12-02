using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Center_3._0_Database.Models;

namespace Student_Center_3._0_Database.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTimeController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public CourseTimeController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/CourseTime
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseTime>>> GetCourseTimes()
        {
            return await _context.CourseTimes.ToListAsync();
        }

        // GET: api/CourseTime/5
        [HttpGet("{courseNum}/{weekday}")]
        public async Task<ActionResult<CourseTime>> GetCourseTime(int courseNum, int weekday)
        {
            var courseTime = await _context.CourseTimes.SingleOrDefaultAsync(ct => ct.courseNum == courseNum && ct.weekday == weekday);

            if (courseTime == null)
            {
                return NotFound();
            }

            return courseTime;
        }

        // GET: api/StudentCourseEnrollment/course/{courseNum}
        [HttpGet("course/{courseNum}")]
        public async Task<ActionResult<List<CourseTime>>> GetTimesByCourses(int courseNum)
        {
            // Fetch all columns for the courses the user is enrolled in
            var courseTimes = await _context.CourseTimes
                .Where(ct => ct.courseNum == courseNum)
                .ToListAsync();

            if (courseTimes == null || !courseTimes.Any())
            {
                return NotFound();
            }

            return courseTimes;
        }

        // PUT: api/CourseTime/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{courseNum}/{weekday}")]
        public async Task<IActionResult> PutCourseTime(int courseNum, int weekday, CourseTime courseTime)
        {
            if (courseNum != courseTime.courseNum || weekday != courseTime.weekday)
            {
                return BadRequest();
            }

            _context.Entry(courseTime).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseTimeExists(courseNum, weekday))
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

        // POST: api/CourseTime
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourseTime>> PostCourseTime(CourseTime courseTime)
        {
            _context.CourseTimes.Add(courseTime);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseTimeExists(courseTime.courseNum, courseTime.weekday))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourseTime", new { courseNum = courseTime.courseNum, weekday = courseTime.weekday }, courseTime);
        }

        // DELETE: api/CourseTime/5
        [HttpDelete("{courseNum}/{weekday}")]
        public async Task<IActionResult> DeleteCourseTime(int courseNum, int weekday)
        {
            var courseTime = await _context.CourseTimes.SingleOrDefaultAsync(ct => ct.courseNum == courseNum && ct.weekday == weekday);

            if (courseTime == null)
            {
                return NotFound();
            }

            _context.CourseTimes.Remove(courseTime);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseTimeExists(int courseNum, int weekday)
        {
            return _context.CourseTimes.Any(ct => ct.courseNum == courseNum && ct.weekday == weekday);
        }
    }
}
