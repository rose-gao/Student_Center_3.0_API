using Microsoft.AspNetCore.Mvc;
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
}
