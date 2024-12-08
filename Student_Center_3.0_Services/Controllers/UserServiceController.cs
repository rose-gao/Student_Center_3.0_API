using Microsoft.AspNetCore.Mvc;
using Student_Center_3._0_Services.DTOs;
using Student_Center_3._0_Services.Services;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UserServiceController : ControllerBase
{
    private readonly UserService _userService;

    public UserServiceController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userNum}")]
    public async Task<IActionResult> GetProfile(int userNum)
    {
        var user = await _userService.GetUserInformation(userNum);

        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDTO user)
    {
        // For each user attribute, make some basic validation checks using the data annotaionts of UserDTO
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // try to add user, incl. more complex validation logic
            var result = await _userService.AddUser(user);

            if (result == "OK")
            {
                return Ok(new { Message = "User successfully added." });
            }

            return BadRequest(new { Message = result });
        }
        catch (Exception ex)
        {
            // Log the exception (optional: using a logging library like Serilog or NLog)
            return StatusCode(500, new { Message = "An internal error occurred.", Details = ex.Message });
        }
    }

}
