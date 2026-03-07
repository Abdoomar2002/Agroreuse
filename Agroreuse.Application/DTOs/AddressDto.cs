namespace Agroreuse.Application.DTOs
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public Guid GovernmentId { get; set; }
        public string? GovernmentName { get; set; }
        public Guid CityId { get; set; }
        public string? CityName { get; set; }
        public string Details { get; set; }
    }
}
