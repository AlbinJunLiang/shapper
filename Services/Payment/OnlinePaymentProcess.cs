namespace Shapper.Services.Payment
{
    public class OnlinePaymentProcess : PaymentProcessTemplate
    {
        public OnlinePaymentProcess(IPaymentStrategy strategy)
            : base(strategy) { }

        public async Task<string> ProcesarPago(
            decimal amount,
            string description,
            string successUrl,
            string cancelUrl
        )
        {
            return await ProcessPayment(amount, description, successUrl, cancelUrl);
        }
    }
}
