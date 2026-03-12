using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class OrderPayment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public string TransactionReference { get; set; }
        public double Subtotal { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaidAt { get; set; }
        public string Status { get; set; }

        // Navegación
        public Order Order { get; set; }
    }
}
