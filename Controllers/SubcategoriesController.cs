using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.Subcategories;
using Shapper.Services.Subcategories;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoryService _subcategoryService;
        private readonly ILogger<SubcategoriesController> _logger;

        public SubcategoriesController(ISubcategoryService subcategoryService, ILogger<SubcategoriesController> logger)
        {
            _subcategoryService = subcategoryService;
            _logger = logger;
        }

        [HttpPost]
        //   [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Create([FromForm] SubcategoryDto dto)
        {
            try
            {
                var subcategory = await _subcategoryService.CreateAsync(dto);
                return Ok(new { success = true, message = "Subcategory created successfully", data = subcategory });
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("already exists"))
                    return Conflict(new { success = false, message = ex.Message });
                if (ex.Message.Contains("does not exist"))
                    return NotFound(new { success = false, message = ex.Message });
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subcategory");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var subcategory = await _subcategoryService.GetByIdAsync(id);
            if (subcategory == null)
                return NotFound(new { success = false, message = "Subcategory not found." });

            return Ok(new { success = true, data = subcategory });
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(new { success = false, message = "Page number must be greater than 0." });

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(new { success = false, message = "Page size must be between 1 and 100." });

            var result = await _subcategoryService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        //   [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Update(int id, [FromForm] SubcategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var subcategory = await _subcategoryService.UpdateAsync(id, dto);
                return Ok(new { success = true, message = "Subcategory updated successfully", data = subcategory });
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
                _logger.LogError(ex, "Error updating subcategory {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        //  [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteAsync(int id, [FromQuery] string? provider)
        {
            try
            {
                await _subcategoryService.DeleteAsync(id, provider);

                return Ok(new { success = true, message = "Subcategory deleted successfully." });
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
                _logger.LogError(ex,
                    "Error deleting subcategory {Id}, Provider: {Provider}",
                    id, provider);

                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }
    }
}