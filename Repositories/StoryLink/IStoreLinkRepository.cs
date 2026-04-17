using Shapper.Models;

namespace Shapper.Repositories.StoreLinks
{
    public interface IStoreLinkRepository
    {
        Task<StoreLink?> GetByIdAsync(int id);
        Task<StoreLink?> GetByNameAndStoreAsync(
            string name,
            int storeInformationId,
            int? excludeId = null
        );
        Task<List<StoreLink>> GetByStoreIdAsync(int storeInformationId);
        Task<(List<StoreLink> StoreLinks, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeInformationId = null
        );
        Task<bool> StoreInformationExistsAsync(int storeInformationId);
        Task<int> CountLinksByStoreAsync(int storeInformationId);
        Task AddAsync(StoreLink storeLink);
        Task<StoreLink> UpdateAsync(StoreLink storeLink);
        Task DeleteAsync(StoreLink storeLink);
        Task SaveChangesAsync();
    }
}
