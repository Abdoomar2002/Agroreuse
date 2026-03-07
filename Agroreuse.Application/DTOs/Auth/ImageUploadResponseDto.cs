namespace Agroreuse.Application.DTOs.Auth
{
    public class ImageUploadResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ImageUrl { get; set; }
    }
}
