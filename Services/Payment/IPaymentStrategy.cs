namespace Shapper.Services.Payment
{
    public interface IPaymentStrategy
    {
        Task<string> CreatePaymentAsync(
            decimal amount,
            string description,
            string successUrl,
            string cancelUrl
        );
        Task<bool> CapturePaymentAsync(string paymentId);
    }
}
