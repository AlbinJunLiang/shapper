using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Shapper.Config;

namespace Shapper.Services.Payment.Strategies
{
    public class PaypalPaymentStrategy : IPaymentStrategy
    {
        private readonly PayPalSettings _paypal;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public PaypalPaymentStrategy(
            IOptions<PayPalSettings> paypal,
            IConfiguration config,
            IHttpClientFactory factory
        )
        {
            _paypal = paypal.Value;
            _config = config;
            _http = factory.CreateClient();
        }

        public async Task<string> CreatePaymentAsync(
            decimal amount,
            string description,
            string successUrl,
            string cancelUrl
        )
        {
            // 1️⃣ Obtener token
            var tokenRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_paypal.Api}/v1/oauth2/token"
            );

            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{_paypal.ClientId}:{_paypal.Secret}")
                )
            );

            tokenRequest.Content = new StringContent(
                "grant_type=client_credentials",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );

            var tokenResponse = await _http.SendAsync(tokenRequest);
            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            var accessToken = JsonDocument
                .Parse(tokenJson)
                .RootElement.GetProperty("access_token")
                .GetString();

            // 2️⃣ Crear orden
            var order = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = "USD",
                            value = amount.ToString("F2", CultureInfo.InvariantCulture),
                        },
                    },
                },
                application_context = new
                {
                    brand_name = "mycompany.com",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = successUrl,
                    cancel_url = cancelUrl,
                },
            };

            var orderRequest = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_paypal.Api}/v2/checkout/orders"
            );

            orderRequest.Headers.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                accessToken
            );
            orderRequest.Content = new StringContent(
                JsonSerializer.Serialize(order),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.SendAsync(orderRequest);
            var resultJson = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(resultJson);

            var approveUrl = doc
                .RootElement.GetProperty("links")
                .EnumerateArray()
                .First(x => x.GetProperty("rel").GetString() == "approve")
                .GetProperty("href")
                .GetString();

            return approveUrl;
        }



        public async Task<bool> CapturePaymentAsync(string paymentId)
        {
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{_paypal.Api}/v2/checkout/orders/{paymentId}/capture"
                );

                // Basic Auth
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{_paypal.ClientId}:{_paypal.Secret}")
                    )
                );

                // Body vacío obligatorio
                request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                var response = await _http.SendAsync(request);

                // Devuelve true si status code 200 o 201
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
