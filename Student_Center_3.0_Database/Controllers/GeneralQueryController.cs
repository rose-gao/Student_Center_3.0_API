using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Student_Center_3._0_Database.Models;

namespace Student_Center_3._0_Database.Controllers
{
    // Used to execute a SELECT query on several tables

    [Route("api/[controller]")]
    [ApiController]
    public class GeneralQueryController : ControllerBase
    {
        private readonly StudentCenterContext _context;
        private readonly IConfiguration _configuration;

        public GeneralQueryController(StudentCenterContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("executeQuery")]
        public async Task<IActionResult> ExecuteQuery([FromBody] string sqlQuery)
        {
            Console.WriteLine(sqlQuery);
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                return BadRequest("SQL query cannot be empty.");
            }

            // Optional: Basic validation to ensure it's a SELECT query
            if (!sqlQuery.Trim().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only SELECT queries are allowed.");
            }

            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("DevConnection")))
                {
                    await connection.OpenAsync();

                    // Using Dapper to execute the query and return results
                    var results = await connection.QueryAsync(sqlQuery);
                    return Ok(results);
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(500, $"Error executing query: {ex.Message}");
            }
        }
    }
}
