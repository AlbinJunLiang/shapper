namespace Shapper.DTOs
{
    public class PaymentRequestDto
    {
        public string Provider { get; set; } = "stripe"; // "stripe" o "paypal"
        public decimal Amount { get; set; }
        public string Description { get; set; } = "";
        public string SuccessUrl { get; set; } = "";
        public string CancelUrl { get; set; } = "";
    }
}
