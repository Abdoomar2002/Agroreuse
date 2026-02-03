using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.DTOs.Auth
{
    public class VerifyOtpRequestDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Otp { get; set; }
        public UserType Type { get; set; }
    }
}
