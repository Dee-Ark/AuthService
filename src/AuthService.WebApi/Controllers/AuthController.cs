using AuthService.Application.DTOs;
using AuthService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var res = await _authService.RegisterAsync(request);
            return Ok(res);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var res = await _authService.LoginAsync(request);
            return Ok(res);
        }
    }
}
