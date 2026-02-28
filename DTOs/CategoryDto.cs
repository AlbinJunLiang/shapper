using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos
{
    public class CategoryDto
    {
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "The Name cannot exceed 500 characters.")]
        public string Description { get; set; }

        [MaxLength(500, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string ImageUrl { get; set; }
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
