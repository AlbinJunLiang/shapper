namespace Shapper.Dtos
{
    public class ProductFilterDto
    {
        public int? CategoryId { get; set; }
        public int? SubcategoryId { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }
}