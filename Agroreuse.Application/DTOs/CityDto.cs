namespace Agroreuse.Application.DTOs
{
    public class CityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid GovernmentId { get; set; }
        public string? GovernmentName { get; set; }
    }
}
