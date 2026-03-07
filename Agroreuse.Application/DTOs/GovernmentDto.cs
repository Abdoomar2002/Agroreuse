namespace Agroreuse.Application.DTOs
{
    public class GovernmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<CityDto>? Cities { get; set; }
    }
}
