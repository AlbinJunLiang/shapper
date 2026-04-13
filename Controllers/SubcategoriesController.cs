using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.Subcategories;
using Shapper.Models;
using Shapper.Services.Subcategories;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubcategoriesController : ControllerBase
    {
        private readonly ISubcategoryService _subcategoryService;

        public SubcategoriesController(ISubcategoryService subcategoryService)
        {
            _subcategoryService = subcategoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubcategoryDto dto)
        {
            try
            {
                var subcategory = await _subcategoryService.CreateAsync(dto);
                return Ok(subcategory);
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("Subcategory name already exists."))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("The specified category does not exist."))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var subcategory = await _subcategoryService.GetByIdAsync(id);
            if (subcategory == null)
                return NotFound(new { message = "Subcategory not found." });

            return Ok(subcategory);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                return BadRequest(
                    new { success = false, message = "Page number must be greater than 0." }
                );

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(
                    new { success = false, message = "Page size must be between 1 and 100." }
                );

            var result = await _subcategoryService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SubcategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var subcategory = await _subcategoryService.UpdateAsync(id, dto);
                return Ok(new { message = "Subcategory updated successfully", data = subcategory });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "Subcategory not found." => NotFound(
                        new { message = ex.Message, status = 404 }
                    ),
                    "Subcategory name already exists." => Conflict(
                        new { message = ex.Message, status = 409 }
                    ),
                    _ => StatusCode(500, new { message = "Internal server error." }),
                };
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _subcategoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
