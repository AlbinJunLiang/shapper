using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.StoreInformations;
using Shapper.Models;
using Shapper.Services.StoreInformations;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreInformationsController : ControllerBase
    {
        private readonly IStoreInformationService _storeInformationService;

        public StoreInformationsController(IStoreInformationService storeInformationService)
        {
            _storeInformationService = storeInformationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(StoreInformationDto dto)
        {
            try
            {
                var storeInformation = await _storeInformationService.CreateAsync(dto);
                return Ok(storeInformation);
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
            var storeInformation = await _storeInformationService.GetByIdAsync(id);
            if (storeInformation == null)
                return NotFound();
            return Ok(storeInformation);
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

            var result = await _storeInformationService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StoreInformationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var storeInformation = await _storeInformationService.UpdateAsync(id, dto);

                return Ok(
                    new
                    {
                        message = "StoreInformation updated successfully",
                        data = storeInformation,
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "StoreInformation not found." => NotFound(
                        new { message = ex.Message, status = 404 }
                    ),
                    "StoreInformation name already exists." => Conflict(
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
                await _storeInformationService.DeleteAsync(id);
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
