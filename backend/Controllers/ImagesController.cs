using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.ImageStorages;
using Shapper.Services.ImageStorage;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ImageStrategyFactory _factory;
        private readonly IConfiguration _configuration;

        public ImagesController(ImageStrategyFactory factory, IConfiguration configuration)
        {
            _factory = factory;
            _configuration = configuration;
        }

        /// <summary>
        /// Sube una imagen usando el proveedor especificado
        /// </summary>
        /// <param name="provider">Proveedor: local, cloudinary (opcional, usa el default de configuración)</param>
        /// 
        [Authorize(Policy = "EmailVerifiedOnly")]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromQuery] string? provider = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file provided or file is empty." });

            try
            {
                // Crear estrategia según el provider especificado
                var strategy = _factory.Create(provider);
                var (path, publicId) = await strategy.UploadImageAsync(file);

                string fullUrl = path.StartsWith("http")
                    ? path
                    : $"{Request.Scheme}://{Request.Host}/{path}";

                return Ok(new
                {
                    Url = fullUrl,
                    PublicId = publicId,
                    Provider = provider ?? _configuration["ImageStorage:DefaultProvider"] ?? "local"
                });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Upload failed: {ex.Message}" });
            }
        }

        /// <summary>
        /// Sube una imagen con proveedor especificado en el body
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("upload-with-provider")]
        public async Task<IActionResult> UploadWithProvider([FromForm] ImageUploadRequestDto request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { error = "No file provided or file is empty." });

            try
            {
                var strategy = _factory.Create(request.Provider);
                var (path, publicId) = await strategy.UploadImageAsync(request.File);

                string fullUrl = path.StartsWith("http")
                    ? path
                    : $"{Request.Scheme}://{Request.Host}/{path}";

                return Ok(new
                {
                    Url = fullUrl,
                    PublicId = publicId,
                    Provider = request.Provider ?? "local"
                });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Upload failed: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtiene la lista de proveedores de almacenamiento disponibles
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("providers")]
        public IActionResult GetAvailableProviders()
        {
            var providers = _factory.GetAvailableProviders();
            var defaultProvider = _configuration["ImageStorage:DefaultProvider"] ?? "local";

            return Ok(new
            {
                providers = providers,
                defaultProvider = defaultProvider
            });
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromQuery] string publicId,
            [FromQuery] string? provider = null)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return BadRequest(new { error = "PublicId is required." });

            try
            {
                var strategy = _factory.Create(provider);
                var deleted = await strategy.DeleteImageAsync(publicId);

                if (!deleted)
                    return BadRequest(new { error = "Could not delete the image." });

                return Ok(new { Message = "Image was deleted successfully." });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Delete failed: {ex.Message}" });
            }
        }
    }
}