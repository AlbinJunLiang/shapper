using System;
using System.Collections.Generic;
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
        [EmailAddress] // Validación extra para asegurar formato de correo
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "ACTIVE"; // Valor por defecto seguro

        [StringLength(300, ErrorMessage = "The address cannot exceed 300 characters.")]
        public string Address { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "The phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        // --- NAVEGACIÓN ---

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        // Inicializamos para evitar NullReferenceException al iterar reviews
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
