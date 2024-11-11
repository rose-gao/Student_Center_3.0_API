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
    public class StudentCourseHistoryController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public StudentCourseHistoryController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/StudentCourseHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentCourseHistory>>> GetStudentCourseHistories()
        {
            return await _context.StudentCourseHistories.ToListAsync();
        }

        // GET: api/StudentCourseHistory/5
        [HttpGet("{userNum}/{course}")]
        public async Task<ActionResult<StudentCourseHistory>> GetStudentCourseHistory(int userNum, string course)
        {
            var studentCourseHistory = await _context.StudentCourseHistories.SingleOrDefaultAsync(sch => sch.userNum == userNum && sch.course == course);


            if (studentCourseHistory == null)
            {
                return NotFound();
            }

            return studentCourseHistory;
        }

        // GET: api/StudentCourseHistory/user/{userId}
        [HttpGet("user/{userNum}")]
        public async Task<ActionResult<IEnumerable<string>>> GetCoursesByUser(int userNum)
        {
            var courses = await _context.StudentCourseHistories
                .Where(sch => sch.userNum == userNum)
                .Select(sch => sch.course)
                .ToListAsync();

            if (courses == null || !courses.Any())
            {
                return NotFound();
            }

            return courses;

        }

        // PUT: api/StudentCourseHistory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{userNum}/{course}")]
        public async Task<IActionResult> PutStudentCourseHistory(int userNum, string course, StudentCourseHistory studentCourseHistory)
        {
            if (userNum != studentCourseHistory.userNum || course != studentCourseHistory.course)
            {
                return BadRequest();
            }

            _context.Entry(studentCourseHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentCourseHistoryExists(userNum, course))
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

        // POST: api/StudentCourseHistory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentCourseHistory>> PostStudentCourseHistory(StudentCourseHistory studentCourseHistory)
        {
            _context.StudentCourseHistories.Add(studentCourseHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentCourseHistoryExists(studentCourseHistory.userNum, studentCourseHistory.course))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStudentCourseHistory", new { id = studentCourseHistory.userNum }, studentCourseHistory);
        }

        // DELETE: api/StudentCourseHistory/5
        [HttpDelete("{userNum}/{course}")]
        public async Task<IActionResult> DeleteStudentCourseHistory(int userNum, string course)
        {
            var studentCourseHistory = await _context.StudentCourseHistories.SingleOrDefaultAsync(sch => sch.userNum == userNum && sch.course == course);


            if (studentCourseHistory == null)
            {
                return NotFound();
            }

            _context.StudentCourseHistories.Remove(studentCourseHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentCourseHistoryExists(int userNum, string course)
        {
            return _context.StudentCourseHistories.Any(e => e.userNum == userNum && e.course == course);
        }

    }
}
