using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
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
                UserName = model?.Email??model.PhoneNumber,
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

        [HttpPost("Farmer/login")]
        public async Task<IActionResult> FarmerLogin([FromBody] FarmerLoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            
            if (user == null)
                return Unauthorized(new { message = "Invalid phone number or password." });

            // Validate that this user is actually a Farmer
            if (user.Type != UserType.Farmer)
                return Unauthorized(new { message = "This account is not registered as a Farmer." });

            // Check if account is locked
            if (user.IsLocked)
                return Unauthorized(new { message = "This account has been locked." });
            
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

            return Unauthorized(new { message = "Invalid phone number or password." });
        }
        [HttpPost("Factory/login")]
        public async Task<IActionResult> FactoryLogin([FromBody] FactoryLoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // Validate that this user is actually a Factory
            if (user.Type != UserType.Factory)
                return Unauthorized(new { message = "This account is not registered as a Factory." });

            // Check if account is locked
            if (user.IsLocked)
                return Unauthorized(new { message = "This account has been locked." });
            
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

            return Unauthorized(new { message = "Invalid email or password." });
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