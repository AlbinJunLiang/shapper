namespace Shapper.Services.Payment
{
    public class PaymentProcessTemplate
    {
        protected IPaymentStrategy _paymentStrategy;

        public PaymentProcessTemplate(IPaymentStrategy strategy)
        {
            _paymentStrategy = strategy;
        }

        public async Task<string> ProcessPayment(
            decimal amount,
            string description,
            string successUrl,
            string cancelUrl
        )
        {
            ValidateAmount(amount);
            var paymentUrl = await _paymentStrategy.CreatePaymentAsync(
                amount,
                description,
                successUrl,
                cancelUrl
            );
            await _paymentStrategy.CapturePaymentAsync(paymentUrl);
            return paymentUrl;
        }

        protected void ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Monto inválido");
        }
    }
}
