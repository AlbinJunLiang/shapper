using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.FeaturedProducts;
using Shapper.Services.FeaturedProducts;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturedProductsController : ControllerBase
    {
        private readonly IFeaturedProductService _featuredProductService;
        private readonly ILogger<FeaturedProductsController> _logger;

        public FeaturedProductsController(
            IFeaturedProductService featuredProductService,
            ILogger<FeaturedProductsController> logger)
        {
            _featuredProductService = featuredProductService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeaturedProductDto dto)
        {
            if (dto.ProductId <= 0)
                return BadRequest(new { success = false, message = "ProductId is required." });

            try
            {
                var result = await _featuredProductService.CreateAsync(dto);
                return Ok(new { success = true, message = "Product featured successfully", data = result });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error featuring product");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _featuredProductService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { success = false, message = "Featured product not found." });

            return Ok(new { success = true, data = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(new { success = false, message = "Page must be greater than 0." });

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(new { success = false, message = "Page size must be between 1 and 100." });

            var result = await _featuredProductService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeaturedProductDto dto)
        {
            try
            {
                var result = await _featuredProductService.UpdateAsync(id, dto);
                return Ok(new { success = true, message = "Featured product updated", data = result });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating featured product");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _featuredProductService.DeleteAsync(id);
                return Ok(new { success = true, message = "Featured product removed." });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting featured product");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }
    }
}