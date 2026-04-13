using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.OrderPayments;
using Shapper.Models;
using Shapper.Services.OrderPayments;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderPaymentsController : ControllerBase
    {
        private readonly IOrderPaymentService _orderPaymentService;

        public OrderPaymentsController(IOrderPaymentService orderPaymentService)
        {
            _orderPaymentService = orderPaymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ConfirmPaymentDto dto)
        {
            try
            {
                var orderPayment = await _orderPaymentService.CreateAsync(dto);
                return Ok(orderPayment);
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
            var orderPayment = await _orderPaymentService.GetByIdAsync(id);
            if (orderPayment == null)
                return NotFound();
            return Ok(orderPayment);
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

            var result = await _orderPaymentService.GetPaginatedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var orderPayment = await _orderPaymentService.UpdateAsync(id, dto);

                return Ok(
                    new { message = "OrderPayment updated successfully", data = orderPayment }
                );
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message switch
                {
                    "OrderPayment not found." => NotFound(
                        new { message = ex.Message, status = 404 }
                    ),
                    "OrderPayment name already exists." => Conflict(
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
                await _orderPaymentService.DeleteAsync(id);
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
