using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.ProductImages;
using Shapper.Services.ProductImages;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductImagesController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        private readonly ILogger<ProductImagesController> _logger;

        public ProductImagesController(
            IProductImageService productImageService,
            ILogger<ProductImagesController> logger)
        {
            _productImageService = productImageService;
            _logger = logger;
        }

        /// <summary>
        /// Crea una imagen de producto (subiendo archivo o solo URL)
        /// </summary>
        [HttpPost]
        //    [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromForm] CreateProductImageDto dto)
        {
            try
            {
                var result = await _productImageService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, new
                {
                    success = true,
                    message = "Product image created successfully.",
                    data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product image");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        /// <summary>
        /// Obtiene una imagen por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _productImageService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"ProductImage with ID {id} not found." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product image {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        /// <summary>
        /// Obtiene todas las imágenes de un producto
        /// </summary>
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            try
            {
                var result = await _productImageService.GetByProductIdAsync(productId);
                return Ok(new { success = true, count = result.Count, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images for product {ProductId}", productId);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        /// <summary>
        /// Actualiza una imagen (subiendo nuevo archivo o nueva URL)
        /// </summary>
        [HttpPut("{id}")]
        //    [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductImageDto dto)
        {
            try
            {
                var result = await _productImageService.UpdateAsync(id, dto);
                return Ok(new
                {
                    success = true,
                    message = "Product image updated successfully.",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product image {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        /// <summary>
        /// Elimina una imagen
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] string? provider)
        {
            try
            {
                var result = await _productImageService.DeleteAsync(id, provider ?? "local");

                if (!result)
                    return NotFound(new { success = false, message = $"ProductImage with ID {id} not found." });

                return Ok(new { success = true, message = "Product image deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product image {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }
    }
}