using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Subcategories
{
    public class SubcategoryResponseDto
    {
        public int Id { get; set; }

        // CS8618 Fixed: Using string.Empty for consistency
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int CategoryId { get; set; }
    }
}