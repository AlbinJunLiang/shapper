using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class FeaturedProduct
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public int DisplayedOrder { get; set; }

        // Navegación
        public Product Product { get; set; }
    }
}
