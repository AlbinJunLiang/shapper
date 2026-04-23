using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.Categories;
using Shapper.Services.Categories;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpPost]
     //   [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromForm] CategoryDto dto)
        {
            try
            {
                var category = await _categoryService.CreateAsync(dto);
                return Ok(new { success = true, message = "Category created successfully", data = category });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already exists"))
                    return Conflict(new { success = false, message = ex.Message });
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new { success = false, message = "Category not found." });

            return Ok(new { success = true, data = category });
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(new { success = false, message = "Page number must be greater than 0." });

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(new { success = false, message = "Page size must be between 1 and 100." });

            var result = await _categoryService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("with-price-range")]
        public async Task<IActionResult> GetCategoriesWithGlobalPriceRange()
        {
            var result = await _categoryService.GetCategoriesWithGlobalPriceRangeAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
     //   [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromForm] CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var category = await _categoryService.UpdateAsync(id, dto);
                return Ok(new { success = true, message = "Category updated successfully", data = category });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(new { success = false, message = ex.Message });
                if (ex.Message.Contains("already exists"))
                    return Conflict(new { success = false, message = ex.Message });
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
     //   [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAsync(int id, [FromQuery] string? provider)
        {
            try
            {
                await _categoryService.DeleteAsync(id, provider ?? "LOCAL");
                return Ok(new { success = true, message = "Category deleted successfully." });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }
    }
}