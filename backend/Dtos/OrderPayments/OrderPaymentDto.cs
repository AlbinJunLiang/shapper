namespace Shapper.Dtos.OrderPayments
{
    public class OrderPaymentDto
    {
        public int OrderId { get; set; }

        // Inicializamos para evitar el Warning CS8618
        public string TransactionReference { get; set; } = string.Empty;

        public double Subtotal { get; set; }

        public double TaxAmount { get; set; }

        public double TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime PaidAt { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
