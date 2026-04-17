using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.StoreLinks
{
    public class StoreLinkDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL is required")]
        [MaxLength(500, ErrorMessage = "URL cannot exceed 500 characters")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; } = string.Empty;

        [RegularExpression(
            "^(ACTIVE|INACTIVE)$",
            ErrorMessage = "Status must be ACTIVE or INACTIVE"
        )]
        public string Status { get; set; } = "ACTIVE";

        [Required(ErrorMessage = "StoreInformationId is required")]
        public int StoreInformationId { get; set; }
    }
}
