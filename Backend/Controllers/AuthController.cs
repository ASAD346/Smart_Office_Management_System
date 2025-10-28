using Microsoft.AspNetCore.Mvc;
using SOMS.API.DTOs;
using SOMS.API.Services;

namespace SOMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(request);

            if (response == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(response);
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "SOMS API is running!", timestamp = DateTime.Now });
        }

        [HttpGet("generate-hash/{password}")]
        public IActionResult GenerateHash(string password)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(new { password, hash });
        }
    }
}
