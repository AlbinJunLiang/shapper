using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Categories
{
    public class CategoryDto
    {
        [MaxLength(150, ErrorMessage = "The Name cannot exceed 150 characters.")]
        public string? Name { get; set; }

        [MaxLength(500, ErrorMessage = "The Name cannot exceed 500 characters.")]
        public string Description { get; set; } = "";

        // Opcional: URL de imagen
        [MaxLength(500, ErrorMessage = "The {0} cannot exceed {1} characters.")]
        public string? ImageUrl { get; set; }

        // Opcional: ID de imagen para almacenamiento (Cloudinary public ID o nombre de archivo)
        public string? ImageId { get; set; }

        // Opcional: Subir archivo en lugar de URL
        public IFormFile? ImageFile { get; set; }

        // Opcional: Proveedor para la imagen (cloudinary, local)
        public string? Provider { get; set; }
    }
}