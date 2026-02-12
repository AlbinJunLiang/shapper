using Microsoft.AspNetCore.Mvc;
using Shapper.Services.ImageStorage;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageStorageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageStorageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var result = await _imageService.UploadImageAsync(file);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string publicId)
        {
            var deleted = await _imageService.DeleteImageAsync(publicId);

            if (!deleted)
                return BadRequest("No se pudo eliminar la imagen");

            return Ok("Imagen eliminada");
        }
    }
}
