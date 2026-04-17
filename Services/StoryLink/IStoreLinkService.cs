using Shapper.Dtos;
using Shapper.Dtos.StoreLinks;

namespace Shapper.Services.StoreLinks
{
    public interface IStoreLinkService
    {
        Task<StoreLinkResponseDto?> GetByIdAsync(int id);
        Task<List<StoreLinkResponseDto>> GetByStoreIdAsync(int storeInformationId);
        Task<PagedResponseDto<StoreLinkResponseDto>> GetPaginatedAsync(int page, int pageSize, int? storeInformationId = null);
        Task<StoreLinkResponseDto> CreateAsync(StoreLinkDto dto);
        Task<StoreLinkResponseDto> UpdateAsync(int id, StoreLinkUpdateDto dto);
        Task DeleteAsync(int id);
        Task<StoreLinkResponseDto> ToggleStatusAsync(int id); // Método adicional útil
    }
}