using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.DTOs.Auth
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public UserType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsLocked { get; set; }
    }
}
