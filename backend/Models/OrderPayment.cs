using System;
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

        // Inicializamos con string.Empty para evitar el Warning CS8618
        public string TransactionReference { get; set; } = string.Empty;

        public double Subtotal { get; set; }
        
        public double? TaxAmount { get; set; }
        
        public double? TotalAmount { get; set; }
        
        // Este ya tiene '?', así que el compilador sabe que puede ser nulo. No necesita cambio.
        public string? PaymentMethod { get; set; }

        public DateTime PaidAt { get; set; }

        // Inicializamos para garantizar que nunca sea nulo al crear el objeto
        public string Status { get; set; } = string.Empty;

        // Navegación
        // Usar 'virtual' es una buena práctica para que EF Core maneje mejor la relación
        public virtual Order Order { get; set; } = null!;
    }
}