using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Services.Products;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto dto)
        {
            try
            {
                var product = await _productService.CreateAsync(dto);
                return Ok(product);
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("The specified product does not exist."))
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(int page = 1, int pageSize = 10)
        {
            var validation = ValidatePagination(page, pageSize);
            if (validation != null)
                return validation;

            var result = await _productService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("store")]
        public async Task<IActionResult> GetStoreProductsAsync(int page = 1, int pageSize = 10)
        {
            var validation = ValidatePagination(page, pageSize);
            if (validation != null)
                return validation;

            var result = await _productService.GetProductsStoreViewAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.UpdateAsync(id, dto);

                return Ok(new { message = "Product updated successfully", data = product });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "Product not found." => NotFound(new { message = ex.Message, status = 404 }),
                    "Product name already exists." => Conflict(
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
                await _productService.DeleteAsync(id);
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

        private IActionResult? ValidatePagination(int page, int pageSize)
        {
            if (page <= 0)
                return BadRequest(
                    new { success = false, message = "Page number must be greater than 0." }
                );

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(
                    new { success = false, message = "Page size must be between 1 and 100." }
                );

            return null;
        }
    }
}
