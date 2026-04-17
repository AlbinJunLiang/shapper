using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class StoreInformation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string StoreCode { get; set; } =
            "ST-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Main location cannot exceed 1000 characters")]
        public string MainLocation { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación (puede ser null)
        public virtual ICollection<StoreLink> StoreLinks { get; set; } = new List<StoreLink>();
    }
}
