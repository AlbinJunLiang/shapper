using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Categories
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<CategoryDto>> GetPaginatedAsync(int page, int pageSize);

        Task<CategoryResponseDto?> CreateAsync(CategoryDto dto);

        Task<CategoryResponseDto> UpdateAsync(int id, CategoryDto dto);

        Task DeleteAsync(int id);
    }
}
