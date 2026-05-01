using System;

namespace Shapper.Dtos.Orders
{
    public class OrderDto
    {
        // Inicializado para evitar CS8618
        public string OrderReference { get; set; } = string.Empty;

        public int? LocationId { get; set; }

        public double Total { get; set; }

        public int? CustomerId { get; set; }

        // Valor por defecto para mantener consistencia
        public string Status { get; set; } = "ACTIVE";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
