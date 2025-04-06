using ApiGateway.Authentication.Model;
using ApiGateway.Authentication.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiGateway.Authentication.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Model.RegisterRequest request)
        {
            var user = new User
            {
                Username = request.Username
            };

            await _authService.Register(user, request.Password);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Model.LoginRequest request)
        {
            var token = await _authService.Authenticate(request.Username, request.Password);
            return Ok(new { token });
        }
    }
}
