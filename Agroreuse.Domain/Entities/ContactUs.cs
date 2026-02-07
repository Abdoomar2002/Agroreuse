using Agroreuse.Domain.Common;
using Agroreuse.Domain.Enums;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Contact Us message entity
    /// </summary>
    public class ContactUs : Entity
    {
        /// <summary>
        /// User ID who submitted the contact form
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// User's full name
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        public string? UserPhone { get; set; }

        /// <summary>
        /// User type (Farmer, Factory, Admin)
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        /// Type of contact message
        /// </summary>
        public ContactType ContactType { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// When the message was submitted
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        /// Whether the message has been read by admin
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// Admin response (optional)
        /// </summary>
        public string? AdminResponse { get; set; }

        /// <summary>
        /// When admin responded (optional)
        /// </summary>
        public DateTime? RespondedAt { get; set; }

        public ContactUs() : base()
        {
        }

        public ContactUs(string userId, string userName, string userEmail, string? userPhone, 
            UserType userType, ContactType contactType, string message) : base()
        {
            UserId = userId;
            UserName = userName;
            UserEmail = userEmail;
            UserPhone = userPhone;
            UserType = userType;
            ContactType = contactType;
            Message = message;
            SubmittedAt = DateTime.UtcNow;
            IsRead = false;
        }
    }
}
