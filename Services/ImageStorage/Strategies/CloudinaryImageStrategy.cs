using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Shapper.Services.ImageStorage.Strategies
{
    public class CloudinaryImageStrategy : IImageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStrategy(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<(string Path, string PublicId)> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "imagenes",
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
                throw new Exception("Error uploading image");

            return (result.SecureUrl!.ToString(), result.PublicId);
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            return result.Result == "ok";
        }
    }
}
