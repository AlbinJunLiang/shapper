using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Faqs
{
    public class FaqDto
    {
        [Required(ErrorMessage = "StoreId is required")]
        public int StoreId { get; set; }

        [Required(ErrorMessage = "Question is required")]
        [MaxLength(500, ErrorMessage = "Question cannot exceed 500 characters")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "Answer is required")]
        [MaxLength(2000, ErrorMessage = "Answer cannot exceed 2000 characters")]
        public string Answer { get; set; } = string.Empty;

        [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be ACTIVE or INACTIVE")]
        public string Status { get; set; } = "ACTIVE";
    }
}