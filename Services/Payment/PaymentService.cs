using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.Services.Payment.Strategies;

namespace Shapper.Services.Payment
{
    public class PaymentService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly PayPalSettings _paypalSettings;

        public PaymentService(
            IOptions<PayPalSettings> paypalSettings,
            IConfiguration config,
            IHttpClientFactory factory
        )
        {
            _paypalSettings = paypalSettings.Value;
            _config = config;
            _httpFactory = factory;
        }

        public IPaymentStrategy GetStrategy(string provider)
        {
            return provider.ToLower() switch
            {
                "paypal" => new PaypalPaymentStrategy(
                    Options.Create(_paypalSettings),
                    _config,
                    _httpFactory
                ),
                "stripe" => new StripePayment(_config["ApiSettings:ApiServer"]),
                _ => throw new ArgumentException("Proveedor de pago no soportado"),
            };
        }
    }
}
