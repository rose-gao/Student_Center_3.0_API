using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Center_3._0_Services.Services;

namespace Student_Center_3._0_Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginServiceController : ControllerBase
    {
        private readonly LoginService _loginService;

        public LoginServiceController(LoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpGet("ValidateLogin")]
        public async Task<IActionResult> ValidateLogin(string userId, string password)
        {
            var userNum = await _loginService.ValidateUserCredentials(userId, password);

            if (userNum != -1)
            {
                return Ok(new { Message = "Login successful", UserNum = userNum});
            }
            else
            {
                return Unauthorized("Invalid credentials");
            }
        }

    }
}
