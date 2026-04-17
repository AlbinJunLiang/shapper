using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string? Name { get; set; }

        [MaxLength(500, ErrorMessage = "The Name cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [MaxLength(500, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string? ImageUrl { get; set; }

        public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    }
}
