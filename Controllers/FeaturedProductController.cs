using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.FeaturedProducts;
using Shapper.Models;
using Shapper.Services.FeaturedProducts;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturedProductsController : ControllerBase
    {
        private readonly IFeaturedProductService _featuredProductService;

        public FeaturedProductsController(IFeaturedProductService featuredProductService)
        {
            _featuredProductService = featuredProductService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeaturedProductDto dto)
        {
            try
            {
                var featuredProduct = await _featuredProductService.CreateAsync(dto);
                return Ok(featuredProduct);
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("The specified subcategory does not exist."))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var featuredProduct = await _featuredProductService.GetByIdAsync(id);
            if (featuredProduct == null)
                return NotFound();
            return Ok(featuredProduct);
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

            var result = await _featuredProductService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FeaturedProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var featuredProduct = await _featuredProductService.UpdateAsync(id, dto);

                return Ok(
                    new { message = "FeaturedProduct updated successfully", data = featuredProduct }
                );
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "FeaturedProduct not found." => NotFound(
                        new { message = ex.Message, status = 404 }
                    ),
                    "FeaturedProduct name already exists." => Conflict(
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
                await _featuredProductService.DeleteAsync(id);
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
