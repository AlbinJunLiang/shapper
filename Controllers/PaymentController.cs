using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.Orders;
using Shapper.Dtos.PaymentRequests;
using Shapper.Services.Checkouts;
using Shapper.Services.Payment;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ICheckoutService _checkoutService;

        public PaymentController(PaymentService paymentService, ICheckoutService checkoutService)
        {
            _paymentService = paymentService;
            _checkoutService = checkoutService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _checkoutService.ProcessAsync(dto);
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

        [HttpPost("capture-payment")]
        public async Task<IActionResult> CapturePayment(
            [FromQuery] string provider,
            [FromQuery] string token
        )
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new { error = "Missing required fields" });
            try
            {
                var strategy = _paymentService.GetStrategy(provider);
                var success = await strategy.CapturePaymentAsync(token);

                return Ok(new { success });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new { error = "Internal server error processing the payment" }
                );
            }
        }
    }
}
