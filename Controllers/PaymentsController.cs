using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Stripe.Checkout;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly ApiSettings _apiSettings;

        public PaymentsController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateSession()
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    SuccessUrl = $"{_apiSettings.ApiServer}/success.html",
                    CancelUrl = $"{_apiSettings.ApiServer}/cancel.html",
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new()
                        {
                            Quantity = 1,
                            PriceData = new()
                            {
                                Currency = "usd",
                                UnitAmount = 2000,
                                ProductData = new() { Name = "Laptop" },
                            },
                        },
                        new()
                        {
                            Quantity = 2,
                            PriceData = new()
                            {
                                Currency = "usd",
                                UnitAmount = 1000,
                                ProductData = new() { Name = "TV" },
                            },
                        },
                    },
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new { url = session.Url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
            }
        }
    }
}
