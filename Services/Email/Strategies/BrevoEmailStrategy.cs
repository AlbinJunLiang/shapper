using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.DTOs;

namespace Shapper.Services
{
    public class BrevoEmailStrategy : IEmailStrategy
    {
        private readonly HttpClient _httpClient;
        private readonly BrevoSettings _settings;

        public BrevoEmailStrategy(HttpClient httpClient, IOptions<BrevoSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task SendAsync(EmailDto message)
        {
            var payload = new
            {
                sender = new { name = message.SenderName, email = message.SenderEmail },
                to = new[] { new { email = message.To } },
                subject = message.Subject,
                htmlContent = message.HtmlContent,
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.brevo.com/v3/smtp/email"
            );

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.Add("api-key", _settings.ApiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Brevo error: {error}");
            }
        }
    }
}
