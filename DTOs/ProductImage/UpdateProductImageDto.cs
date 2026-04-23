using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.ProductImages
{
    public class UpdateProductImageDto
    {
        public IFormFile? ImageFile { get; set; }

        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        public string? ImageUrl { get; set; }

        public string? Provider { get; set; }
    }
}