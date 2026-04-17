namespace Shapper.Dtos.ProductImages
{
    public class ProductImageDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; } // FK hacia Product

        // Usamos string.Empty para mantener el estándar de Shapper
        public string ImageUrl { get; set; } = string.Empty;

        public string ResourceReference { get; set; } = string.Empty;
    }
}
