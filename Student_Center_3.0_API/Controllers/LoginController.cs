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
        [HttpGet("{userId}")]
        public async Task<ActionResult<Login>> GetLogin(string userId)
        {
            var login = await _context.Logins.FindAsync(userId);

            if (login == null)
            {
                return NotFound();
            }

            return login;
        }

        // PUT: api/Login/5
        // Use a DTO to eliminate circular reference b/w Login and Student tables
        [HttpPut("{userId}")]
        public async Task<IActionResult> PutLogin(string userId, LoginDTO loginDTO)
        {
            var login = new Login
            {
                userId = loginDTO.UserId,
                password = loginDTO.Password,
                userNum = loginDTO.userNum,

            };
            
            if (userId != login.userId)
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
                if (!LoginExists(userId))
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
                userId = loginDTO.UserId,
                password = loginDTO.Password,
                userNum = loginDTO.userNum,

            };
            _context.Logins.Add(login);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LoginExists(login.userId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLogin", new { id = login.userNum }, login);
        }

        // DELETE: api/Login/5
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteLogin(string userId)
        {
            var login = await _context.Logins.FindAsync(userId);
            if (login == null)
            {
                return NotFound();
            }

            _context.Logins.Remove(login);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LoginExists(string userId)
        {
            return _context.Logins.Any(e => e.userId == userId);
        }
    }
}
