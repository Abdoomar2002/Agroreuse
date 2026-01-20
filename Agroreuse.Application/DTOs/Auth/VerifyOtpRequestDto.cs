namespace Agroreuse.Application.DTOs.Auth
{
    public class VerifyOtpRequestDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
