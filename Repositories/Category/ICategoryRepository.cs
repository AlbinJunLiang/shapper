using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(int id);
        Task<Category> GetByNameAsync(string name);
        Task<CategoriesWithGlobalPriceRangeDto> GetCategoriesWithGlobalPriceRangeAsync();

        Task<(List<Category> Categories, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
