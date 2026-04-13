using Shapper.Dtos.ProductImages;

namespace Shapper.Dtos.Products
{
    public class ProductStoreView2Dto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; } = 0;

        public double Discount { get; set; } = 0;

        public double TaxAmount { get; set; }

        public double NewPrice { get; set; }
        public int Quantity { get; set; } = 0;

        public string Details { get; set; }

        public string Status { get; set; }

        public string SubcategoryName { get; set; }
        public List<ProductImageDto2> Images { get; set; } = new List<ProductImageDto2>();
    }
}
