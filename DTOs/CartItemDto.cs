using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos
{
    public class CartItemDto
    {
        [Required(ErrorMessage = "El ProductId es requerido")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}