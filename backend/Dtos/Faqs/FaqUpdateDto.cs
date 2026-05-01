using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Faqs
{
    public class FaqUpdateDto
    {
        [MaxLength(500, ErrorMessage = "Question cannot exceed 500 characters")]
        public string? Question { get; set; }

        [MaxLength(2000, ErrorMessage = "Answer cannot exceed 2000 characters")]
        public string? Answer { get; set; }

        [RegularExpression("^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be ACTIVE or INACTIVE")]
        public string? Status { get; set; }
    }
}