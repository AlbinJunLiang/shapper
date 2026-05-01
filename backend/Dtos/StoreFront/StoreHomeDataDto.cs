using Shapper.Dtos.Categories;
using Shapper.Dtos.Products;
using Shapper.Dtos.Store;

namespace Shapper.Dtos.StoreFront
{
    public class StoreHomeDataDto
    {
        public StoreResponseDto? StoreInfo { get; set; }
        public PagedResponseDto<ProductStoreViewDto> Products { get; set; } = new();
        public PagedResponseDto<CategoryResponseDto> Categories { get; set; } = new();
        public CategoriesWithGlobalPriceRangeDto CategoriesWithPriceRange { get; set; } = new();
        public DateTime Timestamp { get; set; }
    }
}