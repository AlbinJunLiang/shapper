namespace Shapper.Dtos
{
    public class OrderPaymentDto
    {
        public int OrderId { get; set; }
        public string TransactionReference { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaidAt { get; set; }
        public string Status { get; set; }
    }

    public class OrderPaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionReference { get; set; }
        public double Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaidAt { get; set; }
        public string Status { get; set; }
    }
}
