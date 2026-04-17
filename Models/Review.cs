using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shapper.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        // Inicializamos con string.Empty para que nunca sea null técnico
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Si el estado puede ser nulo en la DB, dejamos el '?'
        public string? Status { get; set; }

        // --- NAVEGACIÓN ---
        // Usamos virtual para Lazy Loading y null! para el compilador
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
