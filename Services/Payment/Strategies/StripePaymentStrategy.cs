using Shapper.Dtos.Orders;
using Shapper.Services.Payment;
using Stripe.Checkout;

namespace Shapper.Services.Payment.Strategies
{
    public class StripePaymentStrategy : IPaymentStrategy
    {
        public string Name => "stripe";

        public async Task<string> CreatePaymentAsync(
            string successUrl,
            string cancelUrl,
            OrderResponseDto orderResponse
        )
        {
            var lineItems = orderResponse
                .Details.Select(detail => new SessionLineItemOptions
                {
                    Quantity = detail.RequestedQuantity,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)Math.Round(detail.FinalPrice * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = detail.ProductName,
                            Description = detail.Description,
                            Images = string.IsNullOrWhiteSpace(detail.ProductImageUrl)
                                ? null
                                : new List<string> { detail.ProductImageUrl },
                        },
                    },
                })
                .ToList();

            var options = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = lineItems,
                Metadata = new Dictionary<string, string>
                {
                    { "OrderReference", orderResponse.OrderReference },
                },
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url ?? "";
        }

        public Task<bool> CapturePaymentAsync(string paymentId)
        {
            return Task.FromResult(true);
        }
    }
}
