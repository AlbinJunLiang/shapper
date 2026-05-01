namespace Shapper.Dtos.FeaturedProducts
{
    public class FeaturedProductResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public double ProductPrice { get; set; }
    }
}