using Shapper.Models;

namespace Shapper.Repositories.Stores
{
    public interface IStoreRepository
    {
        Task<Store?> GetByIdAsync(int id);

        Task<Store?> GetByStoreCodeAsync(string id);

        Task<Store?> GetByNameAsync(string name);
        Task<Store?> GetByEmailAsync(string email);
        Task<(List<Store> stores, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);

        Task<bool> ExistsAsync(int id);

        Task AddAsync(Store store);
        Task<Store> UpdateAsync(Store store);
        Task DeleteAsync(Store store);
    }
}
