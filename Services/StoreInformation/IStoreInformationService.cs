using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.StoreInformations
{
    public interface IStoreInformationService
    {
        Task<StoreInformationDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<StoreInformationResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        );

        Task<StoreInformationResponseDto?> CreateAsync(StoreInformationDto dto);

        Task<StoreInformationResponseDto> UpdateAsync(int id, StoreInformationDto dto);

        Task DeleteAsync(int id);
    }
}
