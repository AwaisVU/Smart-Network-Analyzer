using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartNetworkAnalyzer.API.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace SmartNetworkAnalyzer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        public AuthController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and Password Are Required");
            }

            var normalizedEmail = request.Email.Trim().ToLower();

            var user = new IdentityUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e=>e.Description));
            }

            return StatusCode(201);
        }
    }
}
