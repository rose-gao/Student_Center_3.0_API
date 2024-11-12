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
    public class CourseAntirequisiteController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public CourseAntirequisiteController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/CourseAntirequisite
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseAntirequisite>>> GetCourseAntirequisites()
        {
            return await _context.CourseAntirequisites.ToListAsync();
        }

        // GET: api/CourseAntirequisite/5
        [HttpGet("{course}/{antirequisite}")]
        public async Task<ActionResult<CourseAntirequisite>> GetCourseAntirequisite(string course, string antirequisite)
        {
            var courseAntirequisite = await _context.CourseAntirequisites.SingleOrDefaultAsync(ca => ca.course == course && ca.antirequisite == antirequisite);

            if (courseAntirequisite == null)
            {
                return NotFound();
            }

            return courseAntirequisite;
        }

        // GET: api/CourseAntirequisite/course/{course}
        [HttpGet("course/{course}")]
        public async Task<ActionResult<List<string>>> GetAntirequisitesByCourse(string course)
        {
            var antirequisites = await _context.CourseAntirequisites
                .Where(ca => ca.course == course)
                .Select(ca => ca.antirequisite)
                .ToListAsync();

            if (!antirequisites.Any())
            {
                return NotFound();
            }

            return antirequisites;
        }

        // POST: api/CourseAntirequisite
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CourseAntirequisite>> PostCourseAntirequisite(CourseAntirequisite courseAntirequisite)
        {
            _context.CourseAntirequisites.Add(courseAntirequisite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CourseAntirequisiteExists(courseAntirequisite.course, courseAntirequisite.antirequisite))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCourseAntirequisite",
                new { course = courseAntirequisite.course, antirequisite = courseAntirequisite.antirequisite },
                courseAntirequisite);

        }

        // DELETE: api/CourseAntirequisite/5
        [HttpDelete("{course}/{antirequisite}")]
        public async Task<IActionResult> DeleteCourseAntirequisite(string course, string antirequisite)
        {
            var courseAntirequisite = await _context.CourseAntirequisites.SingleOrDefaultAsync(ca => ca.course == course && ca.antirequisite == antirequisite);
            if (courseAntirequisite == null)
            {
                return NotFound();
            }

            _context.CourseAntirequisites.Remove(courseAntirequisite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseAntirequisiteExists(string course, string antirequisite)
        {
            return _context.CourseAntirequisites.Any(e => e.course == course && e.antirequisite == antirequisite);
        }
    }
}
