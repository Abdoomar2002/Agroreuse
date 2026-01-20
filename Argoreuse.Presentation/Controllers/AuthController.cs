using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                Address = model.Address,
                PhoneNumber = model.PhoneNumber,
                Type = model.Type,
                CreatedAt = DateTime.UtcNow,
                IsLocked = false,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded) return Ok("Registered!");

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized();

            // Use CheckPasswordAsync instead of PasswordSignInAsync for JWT-based auth
            var isValidPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (isValidPassword)
            {
                var token = _jwtTokenService.GenerateToken(user);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    Email = user.Email,
                    FullName = user.FullName
                });
            }

            return Unauthorized();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Type = user.Type,
                CreatedAt = user.CreatedAt,
                IsLocked = user.IsLocked
            });
        }
    }
}