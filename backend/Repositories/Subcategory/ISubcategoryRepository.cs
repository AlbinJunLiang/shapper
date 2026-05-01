using Shapper.Models;

namespace Shapper.Repositories.Subcategories
{
    public interface ISubcategoryRepository
    {
        Task<Subcategory?> GetByIdAsync(int id);
        Task<Subcategory?> GetByNameAsync(string name);
        Task<(List<Subcategory> Subcategories, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task AddAsync(Subcategory subcategory);
        Task<Subcategory> UpdateAsync(Subcategory subcategory);
        Task DeleteAsync(Subcategory subcategory);
        Task<bool> ExistsAsync(int id);
    }
}