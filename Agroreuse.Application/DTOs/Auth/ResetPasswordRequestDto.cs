using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
        public UserType Type { get; set; }
    }
}
