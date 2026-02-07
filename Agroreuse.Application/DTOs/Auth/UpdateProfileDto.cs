namespace Agroreuse.Application.DTOs.Auth
{
    /// <summary>
    /// DTO for updating user profile (excludes image and password)
    /// </summary>
    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
