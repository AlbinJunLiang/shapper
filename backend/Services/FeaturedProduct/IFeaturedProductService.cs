using Shapper.Dtos;
using Shapper.Dtos.FeaturedProducts;

namespace Shapper.Services.FeaturedProducts
{
    public interface IFeaturedProductService
    {
        Task<FeaturedProductResponseDto?> GetByIdAsync(int id);
        Task<List<FeaturedProductResponseDto>> GetAllAsync();
        Task<PagedResponseDto<FeaturedProductResponseDto>> GetPaginatedAsync(int page, int pageSize);
        Task<FeaturedProductResponseDto> CreateAsync(FeaturedProductDto dto);
        Task<FeaturedProductResponseDto> UpdateAsync(int id, FeaturedProductDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByProductIdAsync(int productId);
        Task<bool> IsProductFeaturedAsync(int productId);
        Task<int> GetFeaturedCountAsync();
    }
}