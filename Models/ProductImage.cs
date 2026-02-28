using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "ProductId is required.")]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; } // FK hacia Product

        [Required(ErrorMessage = "ImageUrl is required.")]
        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        public string ImageUrl { get; set; }

        [MaxLength(250, ErrorMessage = "ResourceReference cannot exceed 250 characters.")]
        public string ResourceReference { get; set; }

        // Propiedad de navegación
        public Product Product { get; set; }
    }
}
