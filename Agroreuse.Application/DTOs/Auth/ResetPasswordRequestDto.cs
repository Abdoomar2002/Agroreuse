namespace Agroreuse.Application.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; }
        public string ResetToken { get; set; }
        public string NewPassword { get; set; }
    }
}
