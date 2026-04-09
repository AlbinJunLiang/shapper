using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? LastName { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "The address cannot exceed 10 characters.")]
        public string Address { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        // Navegación
        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; }
    }
}
