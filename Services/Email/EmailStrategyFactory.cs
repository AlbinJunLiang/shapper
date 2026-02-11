using Shapper.Services.Emails.Strategies;

namespace Shapper.Services.Emails
{
    public class EmailStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public EmailStrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEmailStrategy Get(string provider)
        {
            return provider.ToLower() switch
            {
                "smtp" => _provider.GetRequiredService<SmtpEmailStrategy>(),
                "brevo" => _provider.GetRequiredService<BrevoEmailStrategy>(),
                _ => throw new NotSupportedException("Proveedor no soportado"),
            };
        }
    }
}
