using Shapper.Dtos;
using Shapper.Dtos.StoreInformations;

namespace Shapper.Services.StoreInformations
{
    public interface IStoreInformationService
    {
        Task<StoreInformationResponseDto?> GetByIdAsync(int id);

        // Obtiene lista paginada de tiendas
        Task<PagedResponseDto<StoreInformationResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        );

        Task<StoreInformationResponseDto?> CreateAsync(StoreInformationDto dto);

        Task<StoreInformationResponseDto> UpdateAsync(int id, StoreInformationDto dto);

        // Elimina una tienda (solo si no tiene StoreLinks asociados)
        Task DeleteAsync(int id);
    }
}
