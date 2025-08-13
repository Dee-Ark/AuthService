using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProtectedController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { ok = true, user = User.Identity?.Name ?? "n/a" });
    }
}
