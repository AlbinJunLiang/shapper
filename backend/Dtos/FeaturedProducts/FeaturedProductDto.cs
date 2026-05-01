using System.ComponentModel.DataAnnotations;

namespace Shapper.Dtos.FeaturedProducts
{
    public class FeaturedProductDto
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }
    }
}