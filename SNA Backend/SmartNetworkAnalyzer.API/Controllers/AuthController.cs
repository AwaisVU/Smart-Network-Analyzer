using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartNetworkAnalyzer.API.Contracts;
using Microsoft.AspNetCore.Mvc;
using SmartNetworkAnalyzer.API.Services;

namespace SmartNetworkAnalyzer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtTokenService _tokenService;
        public AuthController(UserManager<IdentityUser> userManager, IJwtTokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email and password are required to login");
            }

            var normalizedEmail = request.Email.Trim().ToLower();

            var user = await _userManager.FindByEmailAsync(normalizedEmail);
            if(user == null)
            {
                return Unauthorized("No Email Found");
            }
            
            var isPassValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPassValid)
            {
                return Unauthorized("Incorrect Password!");
            }

            var (token, expiresAtUtc) = _tokenService.CreateToken(user);
            return Ok(new LoginResponse(token, expiresAtUtc));
        }
    }
}
