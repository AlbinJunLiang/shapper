using Shapper.Dtos;
using Shapper.Dtos.FeaturedProducts;
using Shapper.Models;


namespace Shapper.Services.FeaturedProducts
{
    public interface IFeaturedProductService
    {
        Task<FeaturedProductDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<FeaturedProductResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        );

        Task<FeaturedProductResponseDto?> CreateAsync(FeaturedProductDto dto);

        Task<FeaturedProductResponseDto> UpdateAsync(int id, FeaturedProductDto dto);

        Task DeleteAsync(int id);
    }
}
