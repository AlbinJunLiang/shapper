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
            if (request == null)
                return BadRequest(new { error = "Request inválido" });

            if (string.IsNullOrWhiteSpace(request.Provider))
                return BadRequest(new { error = "Proveedor requerido" });

            if (request.Amount <= 0)
                return BadRequest(new { error = "Monto inválido" });

            try
            {
                var strategy = _paymentService.GetStrategy(request.Provider.Trim());

                var getUrl = await strategy.CreatePaymentAsync(
                    request.Amount,
                    request.Description,
                    request.SuccessUrl,
                    request.CancelUrl
                );

                return Ok(new { getUrl });
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // https://slbkrp3n-5127.use2.devtunnels.ms/api/payment/capture-payment?provider=Paypal
        [HttpPost("capture-payment")]
        public async Task<IActionResult> CapturePayment(
            [FromQuery] string provider,
            [FromQuery] string token
        )
        {
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(token))
                return BadRequest(new { error = "Datos incompletos" });

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
                return StatusCode(500, new { error = "Error interno procesando el pago" });
            }
        }
    }

    /*
    [HttpPost("capture-payment")]
    public async Task<IActionResult> CapturePayment([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { error = "Token inválido" });
    
        try
        {
            // 🔎 1. Buscar la orden en BD
            var order = _db.Orders.FirstOrDefault(x => x.PaymentToken == token);
    
            if (order == null)
                return BadRequest(new { error = "Orden no encontrada" });
    
            if (order.IsPaid)
                return BadRequest(new { error = "Orden ya procesada" });
    
            // 🧠 2. Obtener el provider desde BD (NO del frontend)
            var strategy = _paymentService.GetStrategy(order.Provider);
    
            // 💳 3. Capturar
            var success = await strategy.CapturePaymentAsync(token);
    
            return Ok(new { success });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Error interno procesando el pago" });
        }
    }
    */
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
