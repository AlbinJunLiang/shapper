
namespace Shapper.Dtos.ImageStorages
{
    public class ImageUploadRequestDto
    {
        public IFormFile? File { get; set; }
        public string? Provider { get; set; } = "local"; // local, cloudinary
    }
}