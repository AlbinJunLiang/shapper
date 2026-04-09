using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Services.Reviews;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReviewDto dto)
        {
            try
            {
                var review = await _reviewService.CreateAsync(dto);
                return Ok(review);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("does not exist"))
            {
                // Handles both User and Product not found
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already reviewed"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
                when (ex.Message.Contains("rating must be between"))
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new { message = "An internal error occurred while creating the review." }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null)
                return NotFound();
            return Ok(review);
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

            var result = await _reviewService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("filter")] // Agregamos :int para evitar conflictos de ruta
        public async Task<IActionResult> GetFilteredReviews(
            [FromQuery, BindRequired] int productId, // Obligatorio desde el Query
            [FromQuery] int? userId = 0,
            [FromQuery] string? sortBy = "recent",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(
                    new { success = false, message = "Invalid pagination parameters." }
                );

            // Optional: Manual validation if BindRequired doesn't trigger a 400 automatically
            if (productId <= 0)
            {
                return BadRequest(
                    new { message = "ProductId is required and must be greater than 0." }
                );
            }

            // Si quieres ver TODAS las reseñas del producto sin filtrar por usuario,
            // asegúrate de que userId llegue como null en la URL.
            var filter = new ReviewFilterDto
            {
                ProductId = productId,
                UserId = userId,
                SortBy = sortBy,
            };

            var result = await _reviewService.GetFilteredReviewsAsync(filter, page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewDto dto)
        {
            // 1. Validación de anotaciones de datos (Data Annotations)
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var review = await _reviewService.UpdateAsync(id, dto);

                // Retornamos el objeto actualizado para que Angular refresque la lista
                return Ok(new { message = "Review updated successfully", data = review });
            }
            catch (InvalidOperationException ex)
            {
                // 2. Manejo de excepciones de lógica de negocio
                return ex.Message switch
                {
                    "Review not found." => NotFound(new { message = ex.Message, status = 404 }),

                    // Captura errores de rango de estrellas o validaciones del Service
                    string msg when msg.Contains("rating") => BadRequest(new { message = msg }),

                    // El '_' es el default, y DEBE llevar coma después de cada brazo del switch
                    _ => StatusCode(500, new { message = "An error occurred in the service." }),
                };
            }
            catch (Exception)
            {
                // 3. Captura de errores inesperados (Base de datos, conexión, etc.)
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                await _reviewService.DeleteAsync(id);
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
