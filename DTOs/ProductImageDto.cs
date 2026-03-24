namespace Shapper.Dtos
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; } // FK hacia Product
        public string ImageUrl { get; set; }
        public string ResourceReference { get; set; }
    }

    public class ProductImageDto2
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }
}
