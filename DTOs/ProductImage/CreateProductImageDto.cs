using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.ProductImages
{
    public class CreateProductImageDto
    {
        [Required(ErrorMessage = "ProductId is required.")]
        public int ProductId { get; set; }

        // Opción 1: Subir archivo
        public IFormFile? ImageFile { get; set; }

        // Opción 2: Solo URL
        [MaxLength(500, ErrorMessage = "ImageUrl cannot exceed 500 characters.")]
        public string? ImageUrl { get; set; }

        // Opcional: Proveedor de imagen (local, cloudinary)
        public string? Provider { get; set; }
    }
}