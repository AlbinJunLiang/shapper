namespace Shapper.Dtos
{
    public class FeaturedProductDto
    {
        public int ProductId { get; set; }
        public int DisplayedOrder { get; set; }
    }

    public class FeaturedProductResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int DisplayedOrder { get; set; }
    }
}
