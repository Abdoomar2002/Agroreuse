using Agroreuse.Application.DTOs;
using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileUploadService _fileUploadService;
        private readonly ArgoreuseContext _context;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IFileUploadService fileUploadService,
            ArgoreuseContext context)
        {
            _userManager = userManager;
            _fileUploadService = fileUploadService;
            _context = context;
        }

        /// <summary>
        /// Upload or update user profile image
        /// </summary>
        /// <param name="image">Image file (jpg, jpeg, png, gif, webp - max 5MB)</param>
        /// <returns>Image URL</returns>
        [HttpPut("image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile? image)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new ImageUploadResponseDto
                {
                    Success = false,
                    Message = "User not found."
                });

            // If image is null, remove the current image
            if (image == null || image.Length == 0)
            {
                // Delete existing image if any
                if (!string.IsNullOrEmpty(user.ImagePath))
                {
                    _fileUploadService.DeleteImage(user.ImagePath);
                    user.ImagePath = null;
                    await _userManager.UpdateAsync(user);
                }

                return Ok(new ImageUploadResponseDto
                {
                    Success = true,
                    Message = "Profile image removed.",
                    ImageUrl = null
                });
            }

            // Validate image
            if (!_fileUploadService.IsValidImage(image))
            {
                return BadRequest(new ImageUploadResponseDto
                {
                    Success = false,
                    Message = "Invalid image. Allowed formats: jpg, jpeg, png, gif, webp. Max size: 5MB."
                });
            }

            // Delete old image if exists
            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                _fileUploadService.DeleteImage(user.ImagePath);
            }

            // Upload new image
            var imagePath = await _fileUploadService.UploadImageAsync(image, "profiles");

            // Update user
            user.ImagePath = imagePath;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new ImageUploadResponseDto
                {
                    Success = true,
                    Message = "Profile image updated successfully.",
                    ImageUrl = imagePath
                });
            }

            return BadRequest(new ImageUploadResponseDto
            {
                Success = false,
                Message = "Failed to update profile image."
            });
        }

        /// <summary>
        /// Delete user profile image
        /// </summary>
        [HttpDelete("image")]
        public async Task<IActionResult> DeleteProfileImage()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new ImageUploadResponseDto
                {
                    Success = false,
                    Message = "User not found."
                });

            if (string.IsNullOrEmpty(user.ImagePath))
            {
                return Ok(new ImageUploadResponseDto
                {
                    Success = true,
                    Message = "No profile image to delete."
                });
            }

            // Delete image file
            _fileUploadService.DeleteImage(user.ImagePath);

            // Update user
            user.ImagePath = null;
            await _userManager.UpdateAsync(user);

            return Ok(new ImageUploadResponseDto
            {
                Success = true,
                Message = "Profile image deleted successfully."
            });
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

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

        /// <summary>
        /// Update user profile (excludes image and password)
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

            // Update only provided fields
            if (!string.IsNullOrEmpty(model.FullName))
                user.FullName = model.FullName;

            if (model.Address != null)
                user.Address = model.Address;

            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                // Check if phone number is already used by another user
                var existingUser = await _userManager.FindByNameAsync(model.PhoneNumber);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest(new { Success = false, Message = "Phone number is already in use." });
                }
                user.PhoneNumber = model.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                // Check if email is already used by another user
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest(new { Success = false, Message = "Email is already in use." });
                }
                user.Email = model.Email;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Profile updated successfully.",
                    Data = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        Type = user.Type,
                        CreatedAt = user.CreatedAt,
                        IsLocked = user.IsLocked,
                        ImagePath = user.ImagePath
                    }
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = "Failed to update profile.",
                Errors = result.Errors.Select(e => e.Description)
            });
        }

        /// <summary>
        /// Delete account (marks account as locked)
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

            if (user.IsLocked)
            {
                return Ok(new { Success = true, Message = "Account is already locked." });
            }

            user.IsLocked = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { Success = true, Message = "Account has been locked." });
            }

            return BadRequest(new
            {
                Success = false,
                Message = "Failed to lock account.",
                Errors = result.Errors.Select(e => e.Description)
            });
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            // Validate confirm password
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return BadRequest(new { Success = false, Message = "New password and confirm password do not match." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { Success = false, Message = "User not found." });

            // Verify current password
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                return BadRequest(new { Success = false, Message = "Current password is incorrect." });
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Success = true, Message = "Password changed successfully." });
            }

            return BadRequest(new
            {
                Success = false,
                Message = "Failed to change password.",
                Errors = result.Errors.Select(e => e.Description)
            });
        }
    }
}
