namespace Shapper.Dtos.ProductImages
{
    public class ProductImageResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? ResourceReference { get; set; }  // PublicId de Cloudinary o nombre de archivo
        public DateTime CreatedAt { get; set; }
    }
}