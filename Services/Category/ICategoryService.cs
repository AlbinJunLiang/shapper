using Shapper.Dtos;
using Shapper.Dtos.Categories;
using Shapper.Models;

namespace Shapper.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto?> GetByIdAsync(int id);
        Task<PagedResponseDto<CategoryResponseDto>> GetPaginatedAsync(int page, int pageSize);
        Task<CategoriesWithGlobalPriceRangeDto> GetCategoriesWithGlobalPriceRangeAsync();
        Task<CategoryResponseDto?> CreateAsync(CategoryDto dto);
        Task<CategoryResponseDto> UpdateAsync(int id, CategoryDto dto);
        Task DeleteAsync(int id, string provider);
    }
}