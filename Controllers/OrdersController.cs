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
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentWebhookService _paymentWebhooks;

        public OrdersController(IOrderService orderService, IPaymentWebhookService paymentWebhooks)
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

        [HttpGet("user/{userId}")] // Es mejor práctica pasar el userId en la ruta
        public async Task<IActionResult> GetUserOrders(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            // Validaciones básicas de entrada
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
                var result = await _orderService.GetUserOrdersAsync(userId, 60, page, pageSize);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                // Log ex here
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
        )
        {
            // Validaciones básicas de entrada
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
                var result = await _orderService.GetPaginatedAsync(page, pageSize);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                // Log ex here
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
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

            var status = await _paymentWebhooks.ProcessAsync(dto);

            // 3. Conditional Response
            if (status == "FAILED")
            {
                return BadRequest(
                    new
                    {
                        status,
                        message = "Payment could not be processed. Please verify the order reference or status.",
                    }
                );
            }

            if (status == "AlreadyPaid")
            {
                return BadRequest(
                    new { status, message = "A payment for this order has already been processed." }
                );
            }

            // 4. Success Response
            return Ok(
                new { status, message = "Payment processed and order updated successfully." }
            );
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest(new { success = false, message = "Status is required." });

            try
            {
                var result = await _orderService.UpdateStatusAsync(id, status);

                if (!result)
                    return NotFound(
                        new { success = false, message = $"Order with ID {id} not found." }
                    );

                return Ok(
                    new
                    {
                        success = true,
                        message = $"Order status updated to '{status}' successfully.",
                    }
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
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
    }
}
