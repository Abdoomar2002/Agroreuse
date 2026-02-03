using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.DTOs.Auth
{
    public class ForgotPasswordRequestDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public UserType Type { get; set; }
    }
}
