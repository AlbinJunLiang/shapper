namespace Shapper.Dtos
{
    public class ProductFilterDto
    {
        public List<int>? SubcategoryIds { get; set; }
        public List<int>? CategoryIds { get; set; }

        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
    }
}
