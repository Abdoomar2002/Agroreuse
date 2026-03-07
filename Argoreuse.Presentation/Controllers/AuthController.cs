using Agroreuse.Application.DTOs;
using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Domain.Enums;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ArgoreuseContext _context;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            ArgoreuseContext context)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if(model == null || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.FullName))
                return BadRequest(new { message = "Email, password, and full name are required." });
            if((string.IsNullOrEmpty(model.Email) || model.Type==UserType.Farmer) && string.IsNullOrEmpty(model.PhoneNumber))
                return BadRequest(new { message = "Either email or phone number is required." });
            // Check if email or phone number already exists
            if(model.Type==UserType.Factory && await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Email is already registered." });
            // Check if phone number already exists
            if(model.Type==UserType.Farmer && await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber) )
                return BadRequest(new { message = "Phone number is already registered." });
            var user = new ApplicationUser
            {
                UserName = model?.Email??model.PhoneNumber,
                Email = model.Email,
                FullName = model.FullName,
                AddressNavigation =new() 
                {
                    GovernmentId=model.Address.GovernmentId,
                    CityId=model.Address.CityId,
                    Details=model.Address.Details,
                    
                },
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
        public async Task<IActionResult> Login([FromBody] GeneralLoginDto model)
        {
            var user = null as ApplicationUser;
            if (model == null || (string.IsNullOrEmpty(model.Email) &&string.IsNullOrEmpty(model.Phone))|| string.IsNullOrEmpty(model.Password))
                return BadRequest(new { message = "(Email or phone) and password are required." });
            if(!string.IsNullOrEmpty(model.Email))
            {
            user = await _userManager.FindByEmailAsync(model.Email);
                if(user!=null&& user.Type!=model.UserType)
                    user = null;
            }
            else if(!string.IsNullOrEmpty(model.Phone))
                user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == model.Phone&&model.UserType==u.Type);

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password." });

            // Check if account is blocked
            if (user.IsLocked)
                return StatusCode(403, new { message = "This account has been blocked. Please contact support." });
            
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

            // Load address details if exists
            var address = await _context.Addresses
                .Include(a => a.Government)
                .Include(a => a.City)
                .FirstOrDefaultAsync(a => a.ApplicationUserId == userId);

            AddressDto? addressDto = null;
            if (address != null)
            {
                addressDto = new AddressDto
                {
                    Id = address.Id,
                    GovernmentId = address.GovernmentId,
                    GovernmentName = address.Government.Name,
                    CityId = address.CityId,
                    CityName = address.City.Name,
                    Details = address.Details
                };
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Type = user.Type,
                CreatedAt = user.CreatedAt,
                IsLocked = user.IsLocked,
                ImagePath = user.ImagePath,
                AddressDetails = addressDto
            });
        }
    }
}