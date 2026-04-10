using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos
{
    public class CartRequestDto
    {
        [Required(ErrorMessage = "El carrito no puede estar vacío")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un producto")]
        public List<CartItemDto> Items { get; set; } = new();
    }
}