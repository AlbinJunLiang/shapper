using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/paypal/webhook")]
    public class PaypalWebhookController : ControllerBase
    {
        [HttpPost]
        public IActionResult Receive([FromBody] JsonElement payload)
        {
            var eventType = payload.GetProperty("event_type").GetString();
            switch (eventType)
            {
                case "PAYMENT.CAPTURE.COMPLETED":
                    var captureId = payload.GetProperty("resource").GetProperty("id").GetString();

                    // ✅ Aquí:
                    // - guardas en BD
                    // - marcas orden como pagada
                    // - activas servicio
                    Console.WriteLine("PAGADOOOOOOOOOOOOOOO--------------");
                    break;
            }

            return Ok();
        }
    }
}
