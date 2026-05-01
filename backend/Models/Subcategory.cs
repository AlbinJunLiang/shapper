using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class Subcategory
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name is required.")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "The Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "The Image URL cannot exceed 500 characters.")]
        public string? ImageUrl { get; set; }

        public string? ImageProvider { get; set; }

        // NUEVO: ID de la imagen (opcional)
        public string? ImageId { get; set; }  // Para almacenar PublicId de Cloudinary o nombre de archivo local

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}