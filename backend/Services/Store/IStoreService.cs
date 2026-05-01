using Shapper.Dtos;
using Shapper.Dtos.Store;

namespace Shapper.Services.Stores
{
    public interface IStoreService
    {
        Task<StoreResponseDto?> GetByIdAsync(int id);

        Task<StoreResponseDto?> GetByStoreCodeAsync(string storeCode);

        Task<PagedResponseDto<StoreResponseDto>> GetPaginatedAsync(
        int page,
        int pageSize
    );

        Task<StoreResponseDto?> CreateAsync(StoreDto dto);

        Task<StoreResponseDto> UpdateAsync(int id, StoreDto dto);
        Task DeleteAsync(int id);
    }
}
