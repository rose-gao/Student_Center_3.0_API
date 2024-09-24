using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Center_3._0_API.DTOs;
using Student_Center_3._0_API.Models;

namespace Student_Center_3._0_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public LoginController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/Login
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            return await _context.Logins.ToListAsync();
        }

        // GET: api/Login/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Login>> GetLogin(int id)
        {
            var login = await _context.Logins.FindAsync(id);

            if (login == null)
            {
                return NotFound();
            }

            return login;
        }

        // PUT: api/Login/5
        // Use a DTO to eliminate circular reference b/w Login and Student tables
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogin(int id, LoginDTO loginDTO)
        {
            var login = new Login
            {
                studentNum = loginDTO.StudentNum,
                userId = loginDTO.UserId,
                password = loginDTO.Password

            };
            
            if (id != login.studentNum)
            {
                return BadRequest();
            }

            _context.Entry(login).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginExists(id))
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

        // POST: api/Login
        // Use a DTO to eliminate circular reference b/w Login and Student tables
        [HttpPost]
        public async Task<ActionResult<Login>> PostLogin(LoginDTO loginDTO)
        {
            var login = new Login
            {
                studentNum = loginDTO.StudentNum,
                userId = loginDTO.UserId,
                password = loginDTO.Password

            };
            _context.Logins.Add(login);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LoginExists(login.studentNum))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLogin", new { id = login.studentNum }, login);
        }

        // DELETE: api/Login/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogin(int id)
        {
            var login = await _context.Logins.FindAsync(id);
            if (login == null)
            {
                return NotFound();
            }

            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoginExists(int id)
        {
            return _context.Logins.Any(e => e.studentNum == id);
        }
    }
}
