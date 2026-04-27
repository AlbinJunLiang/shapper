using Shapper.Dtos;
using Shapper.Dtos.FeaturedProducts;

namespace Shapper.Services.FeaturedProducts
{
    public interface IFeaturedProductService
    {
        Task<FeaturedProductResponseDto?> GetByIdAsync(int id);
        Task<PagedResponseDto<FeaturedProductResponseDto>> GetPaginatedAsync(int page, int pageSize);
        Task<FeaturedProductResponseDto?> CreateAsync(FeaturedProductDto dto);
        Task<FeaturedProductResponseDto> UpdateAsync(int id, FeaturedProductDto dto);
        Task DeleteAsync(int id);
    }
}