using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class OrderDetail
    {
        [Key] // Es buena práctica marcar la PK explícitamente
        public int Id { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; } = 0;

        public double Subtotal { get; set; } = 0;

        // Si permites nulos en la DB, usa 'string?' sin inicializar.
        // Si NO permites nulos, usa 'string' con string.Empty.
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        // Navegación
        // Usamos null! para decirle al compilador que EF Core se encargará de llenarlos.
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
