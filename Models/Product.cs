using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name is required.")]
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "The Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater or equal to 0.")]
        public double Price { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater or equal to 0.")]
        public int Quantity { get; set; } = 0;

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public double Discount { get; set; } = 0;

        [MaxLength(1000, ErrorMessage = "Details cannot exceed 1000 characters.")]
        public string Details { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Subcategory is required.")]
        [ForeignKey(nameof(Subcategory))]
        public int SubcategoryId { get; set; } // FK clara

        public Subcategory Subcategory { get; set; } // navegación
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
