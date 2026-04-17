namespace Shapper.Dtos.OrderPayments
{
    public class OrderPaymentResponseDto
    {
        public int Id { get; set; }

        // Inicializamos con string.Empty para que el compilador no marque CS8618
        public string TransactionReference { get; set; } = string.Empty;

        public double Subtotal { get; set; }

        public double TaxAmount { get; set; }

        public double TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime PaidAt { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
