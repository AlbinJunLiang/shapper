using Shapper.Models;

namespace Shapper.Repositories.StoreInformations
{
    public interface IStoreInformationRepository
    {
        Task<StoreInformation?> GetByIdAsync(int id);
        Task<StoreInformation?> GetByNameAsync(string name);
        Task<StoreInformation?> GetByEmailAsync(string email);
        Task<(List<StoreInformation> StoreInformations, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
        Task AddAsync(StoreInformation storeInformation);
        Task<StoreInformation> UpdateAsync(StoreInformation storeInformation);
        Task DeleteAsync(StoreInformation storeInformation);
    }
}
