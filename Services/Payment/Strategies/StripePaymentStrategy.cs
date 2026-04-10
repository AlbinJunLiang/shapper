using Shapper.Services.Payment;
using Stripe.Checkout;

namespace Shapper.Services.Payment.Strategies
{
    public class StripePaymentStrategy : IPaymentStrategy
    {
        public string Name => "stripe";

        private readonly string _apiServer;

        public StripePaymentStrategy(IConfiguration config)
        {
            _apiServer = config["ApiSettings:ApiServer"];
        }

        public async Task<string> CreatePaymentAsync(
            decimal amount,
            string description,
            string successUrl,
            string cancelUrl
        )
        {
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        Quantity = 1,
                        PriceData = new()
                        {
                            Currency = "usd",
                            UnitAmount = (long)(amount * 100),
                            ProductData = new()
                            {
                                Name = description,
                                Images = new List<string>
                                {
                                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSA3bBrEkkpRDeIeigN95qw-25Eq8kVoRlEDw&s", // 👈 aquí
                                },
                            },
                        },
                    },
                },
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url ?? "";
        }

        // Stripe no requiere captura explícita aquí
        public Task<bool> CapturePaymentAsync(string paymentId)
        {
            return Task.FromResult(true);
        }
    }
}
