namespace Shapper.Dtos.OrderPayments
{
    public class OrderPaymentResponseDto
    {
        public int Id { get; set; }
        public string TransactionReference { get; set; }
        public double Subtotal { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaidAt { get; set; }
        public string Status { get; set; }
    }
}
