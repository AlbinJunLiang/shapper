using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Stripe;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;

        public StripeWebhookController(IOptions<StripeSettings> stripeOptions)
        {
            _stripeSettings = stripeOptions.Value;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _stripeSettings.WebhookSecret
                );
            }
            catch (StripeException e)
            {
                Console.WriteLine("Webhook error: " + e.Message);
                return BadRequest();
            }

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    Console.WriteLine("Pago completado ✅");
                    break;

                default:
                    Console.WriteLine($"Evento no manejado: {stripeEvent.Type}");
                    break;
            }

            return Ok(new { received = true });
        }
    }
}
