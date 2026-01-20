using Agroreuse.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Agroreuse.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserType Type  { get; set; }
        public bool IsLocked { get; set; }
        
    }
}
