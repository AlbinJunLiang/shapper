using Microsoft.AspNetCore.Http;

namespace Shapper.Services.ImageStorage
{
    public interface IImageService
    {
        Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
