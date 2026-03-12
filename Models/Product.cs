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
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; } = 0;

        public double TaxAmount { get; set; }

        public int Quantity { get; set; } = 0;

        public double Discount { get; set; } = 0;

        public string Details { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Subcategory is required.")]
        [ForeignKey(nameof(Subcategory))]
        public int SubcategoryId { get; set; } // FK clara

        public Subcategory Subcategory { get; set; } // navegación
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public FeaturedProduct FeaturedProduct { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
