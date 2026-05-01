using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Shapper.Dtos.Reviews;
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

        [Authorize(Policy = "EmailVerifiedOnly")]
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


        [Authorize(Policy = "EmailVerifiedOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReviewDto dto)
        {
            // 1. Validación de anotaciones de datos
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2. Obtener el email del token (usuario autenticado)
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            try
            {
                // 3. Pasar el email al servicio
                var review = await _reviewService.UpdateAsync(id, dto, email);

                return Ok(new { success = true, message = "Review updated successfully", data = review });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [Authorize(Policy = "EmailVerifiedOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Obtener el email del token (usuario autenticado)
                var email = User.FindFirst(ClaimTypes.Email)?.Value;

                if (string.IsNullOrEmpty(email))
                    return Unauthorized(new { success = false, message = "User not authenticated." });

                // Pasar el email al servicio (él se encargará de buscar el userId en BD)
                await _reviewService.DeleteAsync(id, email);

                return Ok(new { success = true, message = "Review deleted successfully." });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }
    }
}
