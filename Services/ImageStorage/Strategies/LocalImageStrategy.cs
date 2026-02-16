using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Shapper.Services.ImageStorage.Strategies
{
    public class LocalImageStrategy : IImageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalImageStrategy(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<(string Path, string PublicId)> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            string relativePath = $"uploads/{uniqueFileName}";

            return (relativePath, uniqueFileName);
        }

        public Task<bool> DeleteImageAsync(string publicId)
        {
            string filePath = Path.Combine(_env.WebRootPath, "uploads", publicId);

            if (!File.Exists(filePath))
                return Task.FromResult(false);

            File.Delete(filePath);
            return Task.FromResult(true);
        }
    }
}
