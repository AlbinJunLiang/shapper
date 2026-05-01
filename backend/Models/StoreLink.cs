using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class StoreLink
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL is required")]
        [MaxLength(500, ErrorMessage = "URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty; // "social", "payment", "support", "legal", "other"

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression(
            "^(ACTIVE|INACTIVE)$",
            ErrorMessage = "Status must be ACTIVE or INACTIVE"
        )]
        public string Status { get; set; } = "ACTIVE";

        [Required(ErrorMessage = "storeId is required")]
        [ForeignKey(nameof(Store))]
        public int StoreId { get; set; }

        // Navegación
        public virtual Store Store { get; set; } = null!;

        // Fechas de auditoría (opcional)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
