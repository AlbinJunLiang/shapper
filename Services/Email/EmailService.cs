using Shapper.DTOs;

namespace Shapper.Services
{
    public class EmailService
    {
        private readonly EmailStrategyFactory _factory;

        public EmailService(EmailStrategyFactory factory)
        {
            _factory = factory;
        }

        public async Task SendAsync(string provider, EmailDto message)
        {
            var strategy = _factory.Get(provider);
            await strategy.SendAsync(message);
        }
    }
}
