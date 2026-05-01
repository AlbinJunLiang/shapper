using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.Dtos.Orders;

namespace Shapper.Services.Payment.Strategies
{
    public class PaypalPaymentStrategy : IPaymentStrategy
    {
        public string Name => "paypal";

        private readonly PayPalSettings _paypal;
        private readonly HttpClient _http;

        public PaypalPaymentStrategy(IOptions<PayPalSettings> paypal, IHttpClientFactory factory)
        {
            _paypal = paypal.Value;
            _http = factory.CreateClient();
        }

        public async Task<string> CreatePaymentAsync(
            string successUrl,
            string cancelUrl,
            OrderResponseDto orderResponse
        )
        {
            var accessToken = await GetAccessTokenAsync();

            var order = BuildOrder(successUrl, cancelUrl, orderResponse);

            var responseJson = await SendRequestAsync(
                HttpMethod.Post,
                $"{_paypal.Api}/v2/checkout/orders",
                accessToken,
                order
            );

            return ExtractApproveUrl(responseJson);
        }

        public async Task<bool> CapturePaymentAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();

                var response = await SendRawRequestAsync(
                    HttpMethod.Post,
                    $"{_paypal.Api}/v2/checkout/orders/{paymentId}/capture",
                    accessToken,
                    "{}"
                );

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_paypal.Api}/v1/oauth2/token");

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{_paypal.ClientId}:{_paypal.Secret}")
                )
            );

            request.Content = new StringContent(
                "grant_type=client_credentials",
                Encoding.UTF8,
                "application/x-www-form-urlencoded"
            );

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString()!;
        }

        private object BuildOrder(
            string successUrl,
            string cancelUrl,
            OrderResponseDto orderResponse
        )
        {
            return new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        custom_id = orderResponse.OrderReference,
                        amount = new
                        {
                            currency_code = "USD",
                            value = orderResponse.Total.ToString(
                                "F2",
                                CultureInfo.InvariantCulture
                            ),
                        },
                    },
                },
                application_context = new
                {
                    brand_name = orderResponse.CompanyName ?? "",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = successUrl,
                    cancel_url = cancelUrl,
                },
            };
        }

        private async Task<string> SendRequestAsync(
            HttpMethod method,
            string url,
            string accessToken,
            object body
        )
        {
            var request = new HttpRequestMessage(method, url);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private async Task<HttpResponseMessage> SendRawRequestAsync(
            HttpMethod method,
            string url,
            string accessToken,
            string body
        )
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await _http.SendAsync(request);
        }

        private string ExtractApproveUrl(string json)
        {
            var doc = JsonDocument.Parse(json);

            return doc
                .RootElement.GetProperty("links")
                .EnumerateArray()
                .First(x => x.GetProperty("rel").GetString() == "approve")
                .GetProperty("href")
                .GetString()!;
        }
    }
}
