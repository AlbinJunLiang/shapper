namespace Shapper.Dtos.OrderPayments
{
    public class OrderPaymentDto
    {
        public int OrderId { get; set; }
        public string TransactionReference { get; set; }
        public double Subtotal { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaidAt { get; set; }
        public string Status { get; set; }
    }
}
