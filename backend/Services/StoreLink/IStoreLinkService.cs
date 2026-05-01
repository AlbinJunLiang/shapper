using Shapper.Dtos;
using Shapper.Dtos.StoreLinks;

namespace Shapper.Services.StoreLinks
{
    public interface IStoreLinkService
    {
        Task<StoreLinkResponseDto?> GetByIdAsync(int id);
        Task<List<StoreLinkResponseDto>> GetByStoreIdAsync(int storeId);
        Task<PagedResponseDto<StoreLinkResponseDto>> GetPaginatedAsync(int page, int pageSize, int? storeId = null);
        Task<StoreLinkResponseDto> CreateAsync(StoreLinkDto dto);
        Task<StoreLinkResponseDto> UpdateAsync(int id, StoreLinkUpdateDto dto);
        Task<StoreLinkResponseDto> UpsertAsync(int? id, StoreLinkUpdateDto dto);
        Task DeleteAsync(int id);
        Task<StoreLinkResponseDto> ToggleStatusAsync(int id);
    }
}