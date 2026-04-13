using Microsoft.Extensions.Options;
using Shapper.Config;

namespace Shapper.Services.PaymentUrlValidators
{
    public class PaymentUrlValidator : IPaymentUrlValidator
    {
        private readonly UrlSettings _settings;

        public PaymentUrlValidator(IOptions<UrlSettings> options)
        {
            _settings = options.Value;
        }

        public bool IsValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return _settings.AllowedHosts.Any(h =>
                url.StartsWith(h, StringComparison.OrdinalIgnoreCase)
            );
        }

        public string GetFullUrl(string path) => $"{path.TrimStart('/')}";
    }
}
