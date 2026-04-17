using System;
using System.Collections.Generic;

namespace Shapper.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Mantenemos null! porque es obligatorio y lo genera el sistema
        public string OrderReference { get; set; } = null!;

        public double Total { get; set; }

        public int? CustomerId { get; set; }

        // Inicializamos con el valor por defecto para evitar warnings
        public string Status { get; set; } = "ACTIVE";

        // Al ser un JSON opcional, el string? está perfecto
        public string? ExtraData { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? LocationId { get; set; }

        // --- NAVEGACIÓN ---

        // virtual ayuda a EF Core con el Lazy Loading
        public virtual User? Customer { get; set; }

        // ¡IMPORTANTE! Las colecciones siempre deben inicializarse como una lista vacía.
        // Esto evita errores si haces un foreach sobre ellas cuando no hay datos.
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } =
            new List<OrderDetail>();

        public virtual ICollection<OrderPayment> OrderPayments { get; set; } =
            new List<OrderPayment>();

        public virtual Location? Location { get; set; }
    }
}
