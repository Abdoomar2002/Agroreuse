using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.DTOs
{
    public class ContactUsDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public UserType UserType { get; set; }
        public ContactType ContactType { get; set; }
        public string Message { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsRead { get; set; }
        public string? AdminResponse { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}
