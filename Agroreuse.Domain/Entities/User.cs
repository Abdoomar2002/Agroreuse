using Agroreuse.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Agroreuse.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        
        /// <summary>
        /// Legacy address field (kept for backward compatibility)
        /// Consider using AddressNavigation instead
        /// </summary>
        public string? Address { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public UserType Type  { get; set; }
        public bool IsLocked { get; set; }

        /// <summary>
        /// Relative path to user's profile image stored in wwwroot
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Navigation property to Address (one-to-one nullable relationship)
        /// </summary>
        public virtual Address? AddressNavigation { get; set; }
    }
}
