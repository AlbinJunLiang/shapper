using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Destacar un producto
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeaturedProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            try
            {
                var result = await _featuredProductService.CreateAsync(dto);
                return StatusCode(201, new { success = true, message = "Product featured successfully", data = result });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already featured"))
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error featuring product {ProductId}", dto.ProductId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obtener producto destacado por ID
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _featuredProductService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"Featured product with ID {id} not found" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured product {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obtener todos los productos destacados
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var results = await _featuredProductService.GetAllAsync();
                return Ok(new { success = true, count = results.Count, data = results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all featured products");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obtener productos destacados paginados
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(new { success = false, message = "Page must be greater than 0" });

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(new { success = false, message = "Page size must be between 1 and 100" });

            try
            {
                var result = await _featuredProductService.GetPaginatedAsync(page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated featured products");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Verificar si un producto está destacado
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> IsProductFeatured(int productId)
        {
            try
            {
                var isFeatured = await _featuredProductService.IsProductFeaturedAsync(productId);
                return Ok(new { success = true, productId, isFeatured });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product {ProductId} is featured", productId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

    
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            try
            {
                var count = await _featuredProductService.GetFeaturedCountAsync();
                return Ok(new { success = true, count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured products count");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

   
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeaturedProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            try
            {
                var result = await _featuredProductService.UpdateAsync(id, dto);
                return Ok(new { success = true, message = "Featured product updated successfully", data = result });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating featured product {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _featuredProductService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { success = false, message = $"Featured product with ID {id} not found" });

                return Ok(new { success = true, message = "Featured product removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting featured product {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }


        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("product/{productId}")]
        public async Task<IActionResult> DeleteByProductId(int productId)
        {
            try
            {
                var deleted = await _featuredProductService.DeleteByProductIdAsync(productId);
                if (!deleted)
                    return NotFound(new { success = false, message = $"Product with ID {productId} is not featured" });

                return Ok(new { success = true, message = "Featured product removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting featured product by product {ProductId}", productId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}