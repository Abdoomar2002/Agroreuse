using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Address = u.Address,
                PhoneNumber = u.PhoneNumber,
                Type = u.Type,
                CreatedAt = u.CreatedAt,
                IsLocked = u.IsLocked
            }).ToList();

            return Ok(userDtos);
        }

        /// <summary>
        /// Block or unblock a user (Admin only)
        /// </summary>
        [HttpPut("{userId}/block")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SetUserBlockStatus(string userId, [FromBody] SetBlockStatusRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

            user.IsLocked = request.IsBlocked;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { Success = false, Message = "Failed to update user status." });

            return Ok(new
            {
                Success = true,
                Message = request.IsBlocked ? "User has been blocked." : "User has been unblocked.",
                Data = new { userId, isBlocked = request.IsBlocked }
            });
        }
        [HttpPut("{userId}/reset-password")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ResetUserPassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var newPassword = "123456Aa"; // In a real application, generate a secure random password
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (!result.Succeeded)
                return BadRequest(new { Success = false, Message = "Failed to reset user password." });
            return Ok(new
            {
                Success = true,
                Message = "User password has been reset.",
                Data = new { userId, newPassword }
            });
        }
    }

    public class SetBlockStatusRequest
    {
        public bool IsBlocked { get; set; }
    }
}
