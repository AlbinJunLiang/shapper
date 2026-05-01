using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; } = 0;

        public double Subtotal { get; set; } = 0;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
