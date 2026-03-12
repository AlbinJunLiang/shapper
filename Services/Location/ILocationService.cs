using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Locations
{
    public interface ILocationService
    {
        Task<LocationDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<LocationResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<LocationResponseDto?> CreateAsync(LocationDto dto);

        Task<LocationResponseDto> UpdateAsync(int id, LocationDto dto);

        Task DeleteAsync(int id);
    }
}
