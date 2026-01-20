using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;
        private readonly IHostEnvironment _environment;

        public PasswordController(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,
            ISmsService smsService,
            IHostEnvironment environment)
        {
            _userManager = userManager;
            _otpService = otpService;
            _smsService = smsService;
            _environment = environment;
        }

        /// <summary>
        /// Request password reset - sends OTP to user's phone number
        /// </summary>
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            
            // Don't reveal if user exists or not for security
            if (user == null)
            {
                return Ok(new ForgotPasswordResponseDto
                {
                    Success = true,
                    Message = "If an account with this email exists, an OTP has been sent to the registered phone number."
                });
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                return BadRequest(new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "No phone number registered for this account."
                });
            }

            // Generate OTP
            var otp = _otpService.GenerateOtp(request.Email);

            // Send OTP via SMS
            var smsMessage = $"Your Agroreuse password reset OTP is: {otp}. Valid for 5 minutes.";
            await _smsService.SendSmsAsync(user.PhoneNumber, smsMessage);

            var response = new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "OTP has been sent to your registered phone number."
            };

            // Include OTP in response only in development mode for testing
            if (_environment.IsDevelopment())
            {
                response.DebugOtp = otp;
                response.Message += " (Debug: OTP included in response for testing)";
            }

            return Ok(response);
        }

        /// <summary>
        /// Verify OTP and get reset token
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Invalid request."
                });
            }

            // Validate OTP
            var isValidOtp = _otpService.ValidateOtp(request.Email, request.Otp);
            if (!isValidOtp)
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired OTP."
                });
            }

            // Invalidate OTP after successful verification
            _otpService.InvalidateOtp(request.Email);

            // Generate password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new VerifyOtpResponseDto
            {
                Success = true,
                Message = "OTP verified successfully.",
                ResetToken = resetToken
            });
        }

        /// <summary>
        /// Reset password using the reset token
        /// </summary>
        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new { Success = false, Message = "Invalid request." });
            }

            var result = await _userManager.ResetPasswordAsync(user, request.ResetToken, request.NewPassword);
            
            if (result.Succeeded)
            {
                return Ok(new { Success = true, Message = "Password has been reset successfully." });
            }

            return BadRequest(new 
            { 
                Success = false, 
                Message = "Failed to reset password.",
                Errors = result.Errors.Select(e => e.Description)
            });
        }
    }
}
