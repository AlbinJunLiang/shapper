using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.Dtos.OrderPayments;
using Shapper.Services.PaymentWebhooks;
using Stripe;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IPaymentWebhookService _paymentWebhooks;

        public StripeWebhookController(
            IOptions<StripeSettings> stripeOptions,
            IPaymentWebhookService paymentWebhooks
        )
        {
            _stripeSettings = stripeOptions.Value;
            _paymentWebhooks = paymentWebhooks;
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

                    if (session == null)
                    {
                        return BadRequest("Invalid session data");
                    }

                    if (session.PaymentStatus != "paid")
                    {
                        return Ok(new { received = true });
                    }

                    if (string.IsNullOrEmpty(session.PaymentIntentId))
                    {
                        return BadRequest("PaymentIntentId is missing");
                    }

                    var paymentIntentId = session.PaymentIntentId;

                    if (
                        session.Metadata == null
                        || !session.Metadata.TryGetValue("OrderReference", out var orderReference)
                    )
                    {
                        return BadRequest("Missing OrderReference");
                    }

                    var result = await _paymentWebhooks.ProcessAsync(
                        new ProcessOrderDto
                        {
                            PaidId = paymentIntentId,
                            Reference = orderReference,
                            PaymentMethod = "STRIPE-CARD",
                        }
                    );

                    Console.WriteLine(
                        $"Pago completado para la orden-----------------------------------------: {orderReference}"
                    );

                    return Ok(new { received = true });

                default:
                    Console.WriteLine($"Unhandled event: {stripeEvent.Type}");
                    break;
            }

            return Ok(new { received = true });
        }
    }
}
