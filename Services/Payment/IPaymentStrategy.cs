namespace Shapper.Services.Payment
{
    public interface IPaymentStrategy
    {
        string Name { get; }
        Task<string> CreatePaymentAsync(
            decimal amount,
            string description,
            string successUrl,
            string cancelUrl
        );
        Task<bool> CapturePaymentAsync(string paymentId);
    }
}
