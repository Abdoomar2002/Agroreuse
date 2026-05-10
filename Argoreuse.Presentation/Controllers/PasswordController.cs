using Agroreuse.Application.DTOs.Auth;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
        private readonly IEmailService _emailService;
        private readonly IHostEnvironment _environment;

        public PasswordController(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,
            ISmsService smsService,
            IEmailService emailService,
            IHostEnvironment environment)
        {
            _userManager = userManager;
            _otpService = otpService;
            _smsService = smsService;
            _emailService = emailService;
            _environment = environment;
        }

        /// <summary>
        /// Request password reset - sends OTP to user's phone number
        /// </summary>
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var user =null as ApplicationUser;
            if (string.IsNullOrEmpty(request.Email)&&string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest("You must enter Email or Phone number");
            }
            if(!string.IsNullOrEmpty(request.Email))
            user = await _userManager.FindByEmailAsync(request.Email);
            else if(!string.IsNullOrEmpty(request.Phone))
                user = await _userManager.FindByNameAsync(request.Phone);
            // Don't reveal if user exists or not for security
            if (user == null || user.Type != request.Type)
            {
                return StatusCode(403,new ForgotPasswordResponseDto
                {
                    Success = true,
                    Message = "Failed to find an account with this credentail"
                });
            }
            string otp="";
            // Generate OTP
            if (user.Type==Agroreuse.Domain.Enums.UserType.Factory)
            otp = _otpService.GenerateOtp(request.Email);
            else if(user.Type==Agroreuse.Domain.Enums.UserType.Farmer)
             otp = _otpService.GenerateOtp(request.Phone);

            // Send OTP via Email for Factory, SMS for Farmer
            if(user.Type==Agroreuse.Domain.Enums.UserType.Factory)
                await _emailService.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP code is: {otp}");
            else if(user.Type==Agroreuse.Domain.Enums.UserType.Farmer)
                await _smsService.SendOtpAsync(user.PhoneNumber, otp);

            var target = user.Type==Agroreuse.Domain.Enums.UserType.Factory ? "email" : "phone number";
            var response = new ForgotPasswordResponseDto
            {
                Success = true,
                Message = $"OTP has been sent to your registered {target}"
            };

            // Include OTP in response only in development mode for testing
           

            return Ok(response);
        }

        /// <summary>
        /// Verify OTP and get reset token
        /// </summary>
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            if(string.IsNullOrEmpty(request.Email)&& string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Email or phone are required."
                });
            }
            var user = null as ApplicationUser;
            if(request.Email!=null) {
            user = await _userManager.FindByEmailAsync(request.Email);
                }
            else if(request.Phone!=null)
            {
                user = await _userManager.FindByNameAsync(request.Phone);
            }
            if (user == null || user.Type != request.Type)
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Invalid request."
                });
            }
            bool isValidOtp = false;
            if(user.Type==Agroreuse.Domain.Enums.UserType.Factory)
            {
                // Validate OTP for email
                isValidOtp = _otpService.ValidateOtp(request.Email, request.Otp);
            }
            else if(user.Type==Agroreuse.Domain.Enums.UserType.Farmer)
            {
                // Validate OTP for phone
                isValidOtp = _otpService.ValidateOtp(request.Phone, request.Otp);
            }
            // Validate OTP
            if (!isValidOtp)
            {
                return BadRequest(new VerifyOtpResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired OTP."
                });
               
            }

            // Invalidate OTP after successful verification
           if(user.Type==Agroreuse.Domain.Enums.UserType.Factory)
                _otpService.InvalidateOtp(request.Email);
              else if(user.Type==Agroreuse.Domain.Enums.UserType.Farmer)
                _otpService.InvalidateOtp(request.Phone);

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
            //validate request
            if(string.IsNullOrEmpty(request.Email)&&string.IsNullOrEmpty(request.Phone))
            {
                return BadRequest(new { Success = false, Message = "Email or Phone are required." });
            }
            var user = null as ApplicationUser;
            if(request.Email!=null)
             user = await _userManager.FindByEmailAsync(request.Email);
            else if(request.Phone!=null)
             user = await _userManager.FindByNameAsync(request.Phone);


            if (user == null||user.Type!=request.Type)
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
