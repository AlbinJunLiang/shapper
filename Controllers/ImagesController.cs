using Microsoft.AspNetCore.Mvc;
using Shapper.DTOs; // <- IMPORTANTE
using Shapper.Services.ImageStorage;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImagesController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var (path, publicId) = await _imageService.UploadImageAsync(file);

            // Formamos la ruta solo si es local
            string fullUrl = path.StartsWith("http")
                ? path
                : $"{Request.Scheme}://{Request.Host}/{path}";

            return Ok(new { Url = fullUrl, PublicId = publicId });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string publicId)
        {
            var deleted = await _imageService.DeleteImageAsync(publicId);

            if (!deleted)
                return BadRequest("No se pudo eliminar la imagen");

            return Ok(new { Message = "image was deleted" });
        }
    }
}
