using Microsoft.AspNetCore.Mvc;
using Shapper.Services.Payment;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            // Seleccionamos la estrategia según el proveedor enviado desde el frontend
            var strategy = _paymentService.GetStrategy(request.Provider);

            var getUrl = await strategy.CreatePaymentAsync(
                request.Amount,
                request.Description,
                request.SuccessUrl,
                request.CancelUrl
            );

            return Ok(new { getUrl });
        }

// https://slbkrp3n-5127.use2.devtunnels.ms/api/payment/capture-payment?provider=Paypal
        [HttpGet("capture-payment")]
        public async Task<IActionResult> CapturePayment(
            [FromQuery] string provider,
            [FromQuery] string token
        )
        {
            if (string.IsNullOrEmpty(provider) || string.IsNullOrEmpty(token))
                return BadRequest("Datos incompletos");

            var strategy = _paymentService.GetStrategy(provider);
            var success = await strategy.CapturePaymentAsync(token);

            return Ok(new { success });
        }
    }

    // Modelos de request
    public class PaymentRequest
    {
        public string Provider { get; set; } = ""; // "paypal" o "stripe"
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public string SuccessUrl { get; set; } = "";
        public string CancelUrl { get; set; } = "";
    }

    public class CaptureRequest
    {
        public string Provider { get; set; } = ""; // "paypal" o "stripe"
        public string PaymentId { get; set; } = "";
    }
}
