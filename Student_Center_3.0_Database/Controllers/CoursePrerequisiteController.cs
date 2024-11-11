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
    public class CoursePrerequisiteController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public CoursePrerequisiteController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/CoursePrerequisite
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoursePrerequisite>>> GetCoursePrerequisites()
        {
            return await _context.CoursePrerequisites.ToListAsync();
        }

        // GET: api/CoursePrerequisite/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CoursePrerequisite>> GetCoursePrerequisite(string id)
        {
            var coursePrerequisite = await _context.CoursePrerequisites.FindAsync(id);

            if (coursePrerequisite == null)
            {
                return NotFound();
            }

            return coursePrerequisite;
        }

        // PUT: api/CoursePrerequisite/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCoursePrerequisite(string id, CoursePrerequisite coursePrerequisite)
        {
            if (id != coursePrerequisite.course)
            {
                return BadRequest();
            }

            _context.Entry(coursePrerequisite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoursePrerequisiteExists(id))
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

        // POST: api/CoursePrerequisite
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CoursePrerequisite>> PostCoursePrerequisite(CoursePrerequisite coursePrerequisite)
        {
            _context.CoursePrerequisites.Add(coursePrerequisite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CoursePrerequisiteExists(coursePrerequisite.course))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCoursePrerequisite", new { id = coursePrerequisite.course }, coursePrerequisite);
        }

        // DELETE: api/CoursePrerequisite/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCoursePrerequisite(string id)
        {
            var coursePrerequisite = await _context.CoursePrerequisites.FindAsync(id);
            if (coursePrerequisite == null)
            {
                return NotFound();
            }

            _context.CoursePrerequisites.Remove(coursePrerequisite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CoursePrerequisiteExists(string id)
        {
            return _context.CoursePrerequisites.Any(e => e.course == id);
        }
    }
}
