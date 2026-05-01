using Shapper.Models;

namespace Shapper.Repositories.StoreLinks
{
    public interface IStoreLinkRepository
    {
        Task<StoreLink?> GetByIdAsync(int id);
        Task<StoreLink?> GetByNameAndStoreAsync(string name, int storeId, int? excludeId = null);
        Task<List<StoreLink>> GetByStoreIdAsync(int storeId);
        Task<(List<StoreLink> StoreLinks, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeId = null
        );
        Task<StoreLink> UpsertAsync(StoreLink storeLink);

        Task<bool> StoreExistsAsync(int storeId);
        Task<int> CountLinksByStoreAsync(int storeId);
        Task AddAsync(StoreLink storeLink);
        Task<StoreLink> UpdateAsync(StoreLink storeLink);
        Task DeleteAsync(StoreLink storeLink);
        Task SaveChangesAsync();
    }
}
