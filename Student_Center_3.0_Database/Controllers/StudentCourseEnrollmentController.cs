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
    public class StudentCourseEnrollmentController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public StudentCourseEnrollmentController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/StudentCourseEnrollment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentCourseEnrollment>>> GetstudentCourseEnrollments()
        {
            return await _context.StudentCourseEnrollments.ToListAsync();
        }

        // GET: api/StudentCourseEnrollment/{userNum}/{courseNum}
        [HttpGet("{userNum}/{courseNum}")]
        public async Task<ActionResult<StudentCourseEnrollment>> GetStudentCourseEnrollment(int userNum, int courseNum)
        {
            var studentCourseEnrollment = await _context.StudentCourseEnrollments
                .SingleOrDefaultAsync(sce => sce.userNum == userNum && sce.courseNum == courseNum);

            if (studentCourseEnrollment == null)
            {
                return NotFound();
            }

            return studentCourseEnrollment;
        }


        // GET: api/StudentCourseEnrollment/user/{userNum}
        [HttpGet("user/{userNum}")]
        public async Task<ActionResult<List<int>>> GetCoursesByUser(int userNum)
        {
            var enrolledCourses = await _context.StudentCourseEnrollments
                .Where(sce => sce.userNum == userNum)
                .Select(sce => sce.courseNum)
                .ToListAsync();

            if (enrolledCourses == null || !enrolledCourses.Any())
            {
                return NotFound();
            }

            return enrolledCourses;
        }

        // POST: api/StudentCourseEnrollment
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentCourseEnrollment>> PostStudentCourseEnrollment(StudentCourseEnrollment studentCourseEnrollment)
        {
            _context.StudentCourseEnrollments.Add(studentCourseEnrollment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentCourseEnrollmentExists(studentCourseEnrollment.userNum, studentCourseEnrollment.courseNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetStudentCourseEnrollment",
                new { userNum = studentCourseEnrollment.userNum, courseNum = studentCourseEnrollment.courseNum },
                studentCourseEnrollment);

        }

        // DELETE: api/StudentCourseEnrollment/5
        [HttpDelete("{userNum}/{courseNum}")]
        public async Task<IActionResult> DeleteStudentCourseEnrollment(int userNum, int courseNum)
        {

            var studentCourseEnrollment = await _context.StudentCourseEnrollments.SingleOrDefaultAsync(sce => sce.userNum == userNum && sce.courseNum == courseNum);
            if (studentCourseEnrollment == null)
            {
                return NotFound();
            }

            _context.StudentCourseEnrollments.Remove(studentCourseEnrollment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool StudentCourseEnrollmentExists(int userNum, int courseNum)
        {
            return _context.StudentCourseEnrollments.Any(sce => sce.userNum == userNum && sce.courseNum == courseNum);
        }
    }
}
