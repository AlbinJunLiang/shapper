using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.DTOs;
using Shapper.Services.Payment;

namespace Shapper.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly ApiSettings _apiSettings;

        public PagosController(IOptions<ApiSettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        [HttpPost("pagar")]
        public async Task<IActionResult> Pagar([FromBody] PaymentRequestDto request)
        {
            IPaymentStrategy strategy = PaymentStrategyFactory.Create(
                request.Provider,
                _apiSettings.ApiServer
            );
            var paymentProcess = new OnlinePaymentProcess(strategy);
            var urlPago = await paymentProcess.ProcesarPago(
                request.Amount,
                request.Description,
                request.SuccessUrl,
                request.CancelUrl
            );

            return Ok(new { url = urlPago });
        }
    }
}
