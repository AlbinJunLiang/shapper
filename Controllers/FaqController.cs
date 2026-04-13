using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.Faqs;
using Shapper.Models;
using Shapper.Services.Faqs;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqsController : ControllerBase
    {
        private readonly IFaqService _faqService;

        public FaqsController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(FaqDto dto)
        {
            try
            {
                var faq = await _faqService.CreateAsync(dto);
                return Ok(faq);
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
            var faq = await _faqService.GetByIdAsync(id);
            if (faq == null)
                return NotFound();
            return Ok(faq);
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

            var result = await _faqService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FaqDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var faq = await _faqService.UpdateAsync(id, dto);

                return Ok(new { message = "Faq updated successfully", data = faq });
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "Faq not found." => NotFound(new { message = ex.Message, status = 404 }),
                    "Faq name already exists." => Conflict(
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
                await _faqService.DeleteAsync(id);
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
