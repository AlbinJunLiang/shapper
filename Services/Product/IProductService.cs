using Shapper.Dtos;

namespace Shapper.Services.Products
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<ProductResponseDto>> GetPaginatedAsync(int page, int pageSize);
        Task<PagedResponseDto<ProductStoreViewDto>> GetProductsStoreViewAsync(
            int page,
            int pageSize
        );

        Task<ProductResponseDto?> CreateAsync(ProductDto dto);

        Task<ProductResponseDto> UpdateAsync(int id, ProductDto dto);

        Task DeleteAsync(int id);
    }
}
