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
        public string Name { get; set; } = string.Empty; 

        public string Description { get; set; } = string.Empty; // Protegido contra null

        public double Price { get; set; } = 0;

        public double TaxAmount { get; set; }

        public int Quantity { get; set; } = 0;

        public double Discount { get; set; } = 0;

        public string Details { get; set; } = string.Empty; // Protegido contra null

        public string Status { get; set; } = string.Empty; // Protegido contra null

        [Required(ErrorMessage = "Subcategory is required.")]
        [ForeignKey(nameof(Subcategory))]
        public int SubcategoryId { get; set; }

        public virtual Subcategory Subcategory { get; set; } = null!;

        public virtual ICollection<ProductImage> ProductImages { get; set; } =
            new List<ProductImage>();

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } =
            new List<OrderDetail>();

        public virtual ICollection<FeaturedProduct> FeaturedProducts { get; set; } = new List<FeaturedProduct>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
