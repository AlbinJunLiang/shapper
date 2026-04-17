using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class Faq
    {
        [Key]
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? Url { get; set; }
        public int? DisplayOrder { get; set; }
        public string? Status { get; set; } = "ACTIVE";
    }
}
