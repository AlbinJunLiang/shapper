using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.Locations;
using Shapper.Models;
using Shapper.Services.Locations;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(LocationDto dto)
        {
            try
            {
                var location = await _locationService.CreateAsync(dto);
                return Ok(location);
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
            var location = await _locationService.GetByIdAsync(id);
            if (location == null)
                return NotFound();
            return Ok(location);
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

            var result = await _locationService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] LocationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var location = await _locationService.UpdateAsync(id, dto);

                return Ok(new { message = "Location updated successfully", data = location });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "Location not found." => NotFound(new { message = ex.Message, status = 404 }),
                    "Location name already exists." => Conflict(
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
                await _locationService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = "Internal Server Error" });
            }
        }
    }
}
