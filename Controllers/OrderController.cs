using System.ComponentModel.DataAnnotations; // Necesario para [MaxLength]
using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Models;
using Shapper.Services.Orders;
using Shapper.Services.PaymentWebhooks;

namespace Shapper.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentWebhookService _paymentWebhooks;

        public OrderController(IOrderService orderService, IPaymentWebhookService paymentWebhooks)
        {
            _orderService = orderService;
            _paymentWebhooks = paymentWebhooks;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _orderService.CreateAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("reference/{reference}")]
        public async Task<ActionResult<OrderResponseDto>> GetByReference(
            [FromRoute]
            [MaxLength(100, ErrorMessage = "The reference cannot exceed 100 characters.")]
                string reference
        )
        {
            var order = await _orderService.GetByReferenceAsync(reference);

            if (order == null)
            {
                return NotFound(
                    new { message = $"Order with reference {reference} was not found." }
                );
            }

            return Ok(order);
        }

        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessOrderDto dto)
        {
            // 1. Basic Validation
            if (dto == null || string.IsNullOrEmpty(dto.Reference))
            {
                return BadRequest(
                    new { status = "Error", message = "Order data and reference are required." }
                );
            }

            var result = await _paymentWebhooks.ProcessAsync(dto);

            // 3. Conditional Response
            if (!result)
            {
                return BadRequest(
                    new
                    {
                        status = "Failure",
                        message = "Payment could not be processed. Please verify the order reference or status.",
                    }
                );
            }

            // 4. Success Response
            return Ok(
                new
                {
                    status = "Success",
                    message = "Payment processed and order updated successfully.",
                }
            );
        }
    }
}
