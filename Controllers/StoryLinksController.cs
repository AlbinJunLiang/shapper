using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.StoreLinks;
using Shapper.Services.StoreLinks;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreLinksController : ControllerBase
    {
        private readonly IStoreLinkService _storeLinkService;
        private readonly ILogger<StoreLinksController> _logger;

        public StoreLinksController(
            IStoreLinkService storeLinkService,
            ILogger<StoreLinksController> logger
        )
        {
            _storeLinkService = storeLinkService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new store link
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StoreLinkDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = GetModelStateErrors() });

            try
            {
                var result = await _storeLinkService.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    new
                    {
                        success = true,
                        message = "Store link created successfully",
                        data = result,
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store link");
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Gets a store link by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid ID." });

            try
            {
                var result = await _storeLinkService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(
                        new { success = false, message = $"StoreLink with ID {id} not found." }
                    );

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store link by ID: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Gets all links for a specific store
        /// </summary>
        [HttpGet("store/{storeInformationId}")]
        public async Task<IActionResult> GetByStoreId(int storeInformationId)
        {
            if (storeInformationId <= 0)
                return BadRequest(
                    new { success = false, message = "Invalid StoreInformation ID." }
                );

            try
            {
                var result = await _storeLinkService.GetByStoreIdAsync(storeInformationId);
                return Ok(
                    new
                    {
                        success = true,
                        count = result.Count,
                        data = result,
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error getting links for store: {StoreId}",
                    storeInformationId
                );
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Gets paginated list of store links (optionally filtered by store)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaginated(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? storeInformationId = null
        )
        {
            if (page <= 0)
                return BadRequest(
                    new { success = false, message = "Page number must be greater than 0." }
                );

            if (pageSize <= 0 || pageSize > 100)
                return BadRequest(
                    new { success = false, message = "Page size must be between 1 and 100." }
                );

            try
            {
                var result = await _storeLinkService.GetPaginatedAsync(
                    page,
                    pageSize,
                    storeInformationId
                );
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated store links");
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Updates a store link
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StoreLinkUpdateDto dto)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid ID." });

            if (!ModelState.IsValid)
                return BadRequest(new { success = false, errors = GetModelStateErrors() });

            try
            {
                var result = await _storeLinkService.UpdateAsync(id, dto);
                return Ok(
                    new
                    {
                        success = true,
                        message = "Store link updated successfully",
                        data = result,
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("not found"))
                    return NotFound(new { success = false, message = ex.Message });

                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store link: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Toggles the status of a store link (ACTIVE ↔ INACTIVE)
        /// </summary>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid ID." });

            try
            {
                var result = await _storeLinkService.ToggleStatusAsync(id);
                return Ok(
                    new
                    {
                        success = true,
                        message = $"Store link status changed to {result.Status}",
                        data = result,
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling store link status: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Deletes a store link
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Invalid ID." });

            try
            {
                await _storeLinkService.DeleteAsync(id);
                return Ok(new { success = true, message = "Store link deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store link: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        private IEnumerable<string> GetModelStateErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        }
    }
}
