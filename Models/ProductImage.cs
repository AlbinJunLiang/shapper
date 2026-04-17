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
        public int ProductId { get; set; }

        [Required(ErrorMessage = "ImageUrl is required.")]
        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        public string ImageUrl { get; set; } = string.Empty; // Protegido

        [MaxLength(250, ErrorMessage = "ResourceReference cannot exceed 250 characters.")]
        public string ResourceReference { get; set; } = string.Empty; // Protegido

        // Propiedad de navegación
        // Usamos virtual para Lazy Loading y null! para el compilador
        public virtual Product Product { get; set; } = null!;
    }
}
