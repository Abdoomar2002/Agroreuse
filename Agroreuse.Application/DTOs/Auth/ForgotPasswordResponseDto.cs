namespace Agroreuse.Application.DTOs.Auth
{
    public class ForgotPasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        
        /// <summary>
        /// Only populated in development mode for testing purposes
        /// </summary>
        public string? DebugOtp { get; set; }
    }
}
