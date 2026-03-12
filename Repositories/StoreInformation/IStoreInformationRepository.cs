using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.StoreInformations
{
    public interface IStoreInformationRepository
    {
        Task<StoreInformation> GetByIdAsync(int id);
        Task<(List<StoreInformation> StoreInformations, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task AddAsync(StoreInformation storeInformation);
        Task<StoreInformation> UpdateAsync(StoreInformation storeInformation);
        Task DeleteAsync(StoreInformation storeInformation);
    }
}
