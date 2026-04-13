using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Subcategories
{
    public class SubcategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}
