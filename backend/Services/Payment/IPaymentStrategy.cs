using Shapper.Dtos.Orders;

namespace Shapper.Services.Payment
{
    public interface IPaymentStrategy
    {
        string Name { get; }

        Task<string> CreatePaymentAsync(
            string successUrl,
            string cancelUrl,
            OrderResponseDto orderResponse
        );
        Task<bool> CapturePaymentAsync(string paymentId);
    }
}
