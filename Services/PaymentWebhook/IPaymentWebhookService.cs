using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;

namespace Shapper.Services.PaymentWebhooks
{
    public interface IPaymentWebhookService
    {
        Task<bool> ProcessAsync(ProcessOrderDto dto);
    }
}
