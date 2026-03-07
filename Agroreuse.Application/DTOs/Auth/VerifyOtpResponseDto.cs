namespace Agroreuse.Application.DTOs.Auth
{
    public class VerifyOtpResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        
        /// <summary>
        /// Token to be used for password reset after OTP verification
        /// </summary>
        public string? ResetToken { get; set; }
    }
}
