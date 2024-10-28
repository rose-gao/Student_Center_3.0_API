using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student_Center_3._0_API.DTOs;
using Student_Center_3._0_API.Models;

// TODO: REFACTOR TO USER

namespace Student_Center_3._0_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public UserController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // Use a DTO to eliminate circular reference b/w Login and User tables
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO userDTO)
        {
            var user = new User
            {
                userNum = userDTO.userNum,
                firstName = userDTO.firstName,
                middleName = userDTO.middleName,
                lastName = userDTO.lastName,
                birthday = userDTO.birthday,
                socialInsuranceNum = userDTO.socialInsuranceNum,
                email = userDTO.email,
                phoneNum = userDTO.phoneNum,
                streetAddress = userDTO.streetAddress,
                city = userDTO.city,
                province = userDTO.province,
                postalCode = userDTO.postalCode,
                isAdmin = userDTO.isAdmin

            };

            if (id != user.userNum)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/User
        // Use a DTO to eliminate circular reference b/w Login and User tables
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDTO userDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var user = new User
                {
                    userNum = userDTO.userNum,
                    firstName = userDTO.firstName,
                    middleName = userDTO.middleName,
                    lastName = userDTO.lastName,
                    birthday = userDTO.birthday,
                    socialInsuranceNum = userDTO.socialInsuranceNum,
                    email = userDTO.email,
                    phoneNum = userDTO.phoneNum,
                    streetAddress = userDTO.streetAddress,
                    city = userDTO.city,
                    province = userDTO.province,
                    postalCode = userDTO.postalCode,
                    isAdmin = userDTO.isAdmin

                };

                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users ON");
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users OFF");
                    await transaction.CommitAsync();
                    return CreatedAtAction("GetUser", new { id = user.userNum }, user);
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.Message}");
                }

            }
                        
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.userNum == id);
        }
    }
}
