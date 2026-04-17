using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.Orders
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "El ProductId es requerido")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }

        // CS8618: Non-nullable property 'ProductImageUrl' must contain a non-null value.
        // Solución: Inicializar con string.Empty
        public string? ProductImageUrl { get; set; } = string.Empty;

        // CS8618: Non-nullable property 'Description' must contain a non-null value.
        // Solución: Inicializar con string.Empty
        public string Description { get; set; } = string.Empty;
    }
}
