using Shapper.Dtos;
using Shapper.Dtos.Products;

namespace Shapper.Services.Products
{
    public interface IProductService
    {
        Task<ProductStoreView2Dto?> GetByIdAsync(int id);

        Task<PagedResponseDto<ProductResponseDto>> GetPaginatedAsync(int page, int pageSize);
        Task<PagedResponseDto<ProductStoreViewDto>> GetProductsStoreViewAsync(
            int page,
            int pageSize,
            bool onlyFeatured
        );

        Task<List<ProductStoreViewDto>> SearchProductsAsync(string searchTerm, int count = 5);

        Task<PagedResponseDto<ProductStoreViewDto>> GetFilteredProductsAsync(
            ProductFilterDto filter,
            int page,
            int pageSize
        );

        Task<ProductResponseDto?> CreateAsync(ProductDto dto);

        Task<ProductResponseDto> UpdateAsync(int id, ProductDto dto);

        Task DeleteAsync(int id);
    }
}
