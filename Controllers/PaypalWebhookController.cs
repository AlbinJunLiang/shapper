using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Shapper.Dtos.OrderPayments;
using Shapper.Helpers;
using Shapper.Services.PaymentWebhooks;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/paypal")]
    public class PaypalWebhookController : ControllerBase
    {
        private readonly IPaymentWebhookService _paymentWebhooks;

        public PaypalWebhookController(IPaymentWebhookService paymentWebhooks)
        {
            _paymentWebhooks = paymentWebhooks;
        }

        [Consumes("application/json")]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            Request.EnableBuffering();
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
            }

            Console.WriteLine(body);

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            var eventType = JsonHelper.GetString(root, "event_type");
            if (eventType == null)
                return BadRequest("event_type is required");

            if (eventType != "PAYMENT.CAPTURE.COMPLETED")
                return Ok(new { received = true });

            var resource = JsonHelper.GetObject(root, "resource");
            if (resource == null)
                return BadRequest("resource is required");

            var captureId = JsonHelper.GetString(resource.Value, "id");
            if (captureId == null)
                return BadRequest("resource.id is required");

            var customId = JsonHelper.GetString(resource.Value, "custom_id");
            if (customId == null)
                return BadRequest("resource.custom_id is required");

            var result = await _paymentWebhooks.ProcessAsync(
                new ProcessOrderDto
                {
                    PaidId = captureId,
                    Reference = customId,
                    PaymentMethod = "PAYPAL-CARD",
                }
            );

            Console.WriteLine($"Payment completed for order: {customId}");

            return Ok(new { result });
        }
    }
}
