using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Subcategories
{
    public class SubcategoryDto
    {
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "The Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "The ImageUrl cannot exceed 500 characters.")]
        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }
    }
}