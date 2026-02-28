using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.Services.Payment.Strategies;

namespace Shapper.Services.Payment
{
    public class PaymentService
    {
        private readonly Dictionary<string, IPaymentStrategy> _strategies;

        public PaymentService(IEnumerable<IPaymentStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);
        }

        public IPaymentStrategy GetStrategy(string provider)
        {
            if (!_strategies.TryGetValue(provider, out var strategy))
                throw new NotSupportedException($"Proveedor '{provider}' no disponible");

            return strategy;
        }
    }
}
