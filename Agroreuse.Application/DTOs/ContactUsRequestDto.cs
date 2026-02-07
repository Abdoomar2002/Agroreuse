using Agroreuse.Domain.Enums;

namespace Agroreuse.Application.DTOs
{
    public class ContactUsRequestDto
    {
        public ContactType ContactType { get; set; }
        public string Message { get; set; }
    }
}
