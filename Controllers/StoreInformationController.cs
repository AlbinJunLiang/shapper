using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.StoreInformations;
using Shapper.Services.StoreInformations;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreInformationsController : ControllerBase
    {
        private readonly IStoreInformationService _storeInformationService;
        private readonly ILogger<StoreInformationsController> _logger;

        public StoreInformationsController(
            IStoreInformationService storeInformationService,
            ILogger<StoreInformationsController> logger
        )
        {
            _storeInformationService = storeInformationService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new store information (Location is optional)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StoreInformationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v =>
                            v.Errors.Select(e => e.ErrorMessage)
                        ),
                    }
                );

            try
            {
                var result = await _storeInformationService.CreateAsync(dto);

                // CS8602 Fixed: Verificamos si el resultado es nulo antes de acceder a .Id
                if (result == null)
                {
                    return StatusCode(
                        500,
                        new { success = false, message = "Failed to create store information." }
                    );
                }

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    new { success = true, data = result }
                );
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store information");
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "An internal error occurred while creating the store.",
                    }
                );
            }
        }

        /// <summary>
        /// Gets store information by ID (includes Location and StoreLinks if they exist)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(
                    new { success = false, message = "Invalid ID. ID must be greater than 0." }
                );

            try
            {
                var result = await _storeInformationService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(
                        new
                        {
                            success = false,
                            message = $"StoreInformation with ID {id} not found.",
                        }
                    );

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store information by ID: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Gets paginated list of store information
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPaginated(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            // Validaciones de paginación
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
                var result = await _storeInformationService.GetPaginatedAsync(page, pageSize);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated store information");
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred." }
                );
            }
        }

        /// <summary>
        /// Updates an existing store information
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StoreInformationDto dto)
        {
            if (id <= 0)
                return BadRequest(
                    new { success = false, message = "Invalid ID. ID must be greater than 0." }
                );

            if (!ModelState.IsValid)
                return BadRequest(
                    new
                    {
                        success = false,
                        errors = ModelState.Values.SelectMany(v =>
                            v.Errors.Select(e => e.ErrorMessage)
                        ),
                    }
                );

            try
            {
                var result = await _storeInformationService.UpdateAsync(id, dto);
                return Ok(
                    new
                    {
                        success = true,
                        message = "Store information updated successfully",
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
                _logger.LogError(ex, "Error updating store information with ID: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred while updating." }
                );
            }
        }

        /// <summary>
        /// Deletes a store information (only if it has no associated links)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(
                    new { success = false, message = "Invalid ID. ID must be greater than 0." }
                );

            try
            {
                await _storeInformationService.DeleteAsync(id);
                return Ok(
                    new { success = true, message = "Store information deleted successfully" }
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
                _logger.LogError(ex, "Error deleting store information with ID: {Id}", id);
                return StatusCode(
                    500,
                    new { success = false, message = "An internal error occurred while deleting." }
                );
            }
        }
    }
}
