using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shapper.Config;

/**
TESTING
sandboxito2000@gmail.com
12345678
*/
namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/paypal")]
    public class PaypalController : ControllerBase
    {
        private readonly PayPalSettings _paypal;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public PaypalController(
            IOptions<PayPalSettings> paypal,
            IConfiguration config,
            IHttpClientFactory factory
        )
        {
            _paypal = paypal.Value;
            _config = config;
            _http = factory.CreateClient();
        }

        // ================= 1. CREATE ORDER =================
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder()
        {
            // 🔹 Obtener token
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

            // 🔹 Orden
            var order = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new { amount = new { currency_code = "USD", value = "105.70" } },
                },
                application_context = new
                {
                    brand_name = "mycompany.com",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = $"{_config["ApiSettings:ApiServer"]}/api/paypal/capture-order",
                    cancel_url = $"{_config["ApiSettings:ApiServer"]}/api/paypal/cancel-order",
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
            var result = await response.Content.ReadAsStringAsync();

            return Ok(JsonDocument.Parse(result));
        }

        // ================= 2. CAPTURE =================
        [HttpGet("capture-order")]
        public async Task<IActionResult> CaptureOrder([FromQuery] string token)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_paypal.Api}/v2/checkout/orders/{token}/capture"
            );

            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{_paypal.ClientId}:{_paypal.Secret}")
                )
            );

            // 🔹 CAMBIO CLAVE: body vacío obligatorio
            request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

            await _http.SendAsync(request);
            return Redirect("/success.html");
        }

        // ================= 3. CANCEL =================
        [HttpGet("cancel-order")]
        public IActionResult CancelOrder()
        {
            return Redirect("/cancel.html");
        }
    }
}
