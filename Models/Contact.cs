using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shapper.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [StringLength(10, ErrorMessage = "The address cannot exceed 10 characters.")]
        [Required]
        public string Address { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public int UserId { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}
