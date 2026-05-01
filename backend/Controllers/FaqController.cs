using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.Faqs;
using Shapper.Services.Faqs;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqsController : ControllerBase
    {
        private readonly IFaqService _faqService;
        private readonly ILogger<FaqsController> _logger;

        public FaqsController(IFaqService faqService, ILogger<FaqsController> logger)
        {
            _faqService = faqService;
            _logger = logger;
        }

        /// <summary>
        /// Crear una nueva FAQ
        /// </summary>

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FaqDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });

            try
            {
                var result = await _faqService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id },
                    new { success = true, message = "FAQ created successfully", data = result });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating FAQ");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obtener FAQ por ID
        /// </summary>
        /// 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _faqService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { success = false, message = $"FAQ with ID {id} not found" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting FAQ {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }


        /// <summary>
        /// Obtener FAQs paginados por StoreCode (código de tienda)
        /// </summary>
        /// 

        [HttpGet]
        public async Task<IActionResult> GetPaginatedByStoreCode(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? storeCode = null)
        {
            if (page <= 0)
                return BadRequest(new { success = false, message = "Page must be greater than 0" });

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(new { success = false, message = "Page size must be between 1 and 100" });

            try
            {
                var result = await _faqService.GetPaginatedByStoreCodeAsync(page, pageSize, storeCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated FAQs by store code");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Actualizar FAQ
        /// </summary>
        /// 
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FaqUpdateDto dto)
        {
            try
            {
                var result = await _faqService.UpdateAsync(id, dto);
                return Ok(new { success = true, message = "FAQ updated successfully", data = result });
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
                _logger.LogError(ex, "Error updating FAQ {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }



        /// <summary>
        /// Eliminar FAQ
        /// </summary>

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _faqService.DeleteAsync(id);
                return Ok(new { success = true, message = "FAQ deleted successfully" });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }
    }
}