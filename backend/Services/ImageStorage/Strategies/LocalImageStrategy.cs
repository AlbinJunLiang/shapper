namespace Shapper.Services.ImageStorage.Strategies
{
    public class LocalImageStrategy : IImageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalImageStrategy(
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string Path, string PublicId)> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            string fullUrl = $"{baseUrl}/uploads/{uniqueFileName}";

            return (fullUrl, uniqueFileName);
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