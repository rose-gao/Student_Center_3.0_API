﻿using System;
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
    public class StudentController : ControllerBase
    {
        private readonly StudentCenterContext _context;

        public StudentController(StudentCenterContext context)
        {
            _context = context;
        }

        // GET: api/Student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        // PUT: api/Student/5
        // Use a DTO to eliminate circular reference b/w Login and Student tables
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, StudentDTO studentDTO)
        {
            var student = new Student
            {
                studentNum = studentDTO.studentNum,
                firstName = studentDTO.firstName,
                middleName = studentDTO.middleName,
                lastName = studentDTO.lastName,
                birthday = studentDTO.birthday,
                socialInsuranceNum = studentDTO.socialInsuranceNum,
                email = studentDTO.email,
                phoneNum = studentDTO.phoneNum,
                streetAddress = studentDTO.streetAddress,
                city = studentDTO.city,
                province = studentDTO.province,
                postalCode = studentDTO.postalCode

            };

            if (id != student.studentNum)
            {
                return BadRequest();
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Student
        // Use a DTO to eliminate circular reference b/w Login and Student tables
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent(StudentDTO studentDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                var student = new Student
                {
                    studentNum = studentDTO.studentNum,
                    firstName = studentDTO.firstName,
                    middleName = studentDTO.middleName,
                    lastName = studentDTO.lastName,
                    birthday = studentDTO.birthday,
                    socialInsuranceNum = studentDTO.socialInsuranceNum,
                    email = studentDTO.email,
                    phoneNum = studentDTO.phoneNum, 
                    streetAddress = studentDTO.streetAddress,
                    city = studentDTO.city,
                    province = studentDTO.province,
                    postalCode = studentDTO.postalCode                   

                };

                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students ON");
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Students OFF");
                    await transaction.CommitAsync();
                    return CreatedAtAction("GetStudent", new { id = student.studentNum }, student);
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Internal server error: {ex.Message} - {ex.InnerException?.Message}");
                }

            }
                        
        }

        // DELETE: api/Student/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.studentNum == id);
        }
    }
}
