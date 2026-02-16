using Shapper.Services.Payment;
using Stripe.Checkout;

namespace Shapper.Services.Payment.Strategies
{
    public class StripePayment : IPaymentStrategy
    {
        private readonly string _apiServer;

        public StripePayment(string apiServer)
        {
            _apiServer = apiServer;
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
                            ProductData = new() { Name = description },
                        },
                    },
                },
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url ?? "";
        }

        public Task<bool> CapturePaymentAsync(string paymentId)
        {
            // Stripe no requiere captura explícita aquí
            return Task.FromResult(true);
        }
    }
}
