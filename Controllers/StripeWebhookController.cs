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
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (session.PaymentStatus == "paid")
                    {
                        string paymentIntentId = session.PaymentIntentId;
                        Console.WriteLine($"Pago completado {session.PaymentStatus}");
                        Console.WriteLine($"Stripe Payment Intent ID (PaidId): {paymentIntentId}");
                        if (session.Metadata.TryGetValue("OrderReference", out var orderReference))
                        {
                            Console.WriteLine($"Pago completado para la orden: {orderReference}");

                            // Ahora puedes llamar a tu servicio para actualizar la orden en Shapper
                            // await _orderService.UpdateStatusToPaidAsync(orderReference);
                        }
                        else
                        {
                            Console.WriteLine(
                                "La sesión de Stripe no contiene la referencia de la orden."
                            );
                        }
                    }
                    break;

                default:
                    Console.WriteLine($"Evento no manejado: {stripeEvent.Type}");
                    break;
            }

            return Ok(new { received = true });
        }
    }
}
