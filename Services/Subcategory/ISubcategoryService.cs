using Shapper.Dtos;
using Shapper.Dtos.Subcategories;
using Shapper.Models;

namespace Shapper.Services.Subcategories
{
    public interface ISubcategoryService
    {
        Task<SubcategoryDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<SubcategoryResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<SubcategoryResponseDto?> CreateAsync(SubcategoryDto dto);

        Task<SubcategoryResponseDto> UpdateAsync(int id, SubcategoryDto dto);

        Task DeleteAsync(int id);
    }
}
