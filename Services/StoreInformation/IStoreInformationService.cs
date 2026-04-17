using Shapper.Dtos;
using Shapper.Dtos.StoreInformations;

namespace Shapper.Services.StoreInformations
{
    public interface IStoreInformationService
    {
        // Obtiene una tienda por ID (incluye Location y StoreLinks si existen)
        Task<StoreInformationResponseDto?> GetByIdAsync(int id);
        
        // Obtiene lista paginada de tiendas
        Task<PagedResponseDto<StoreInformationResponseDto>> GetPaginatedAsync(int page, int pageSize);
        
        // Crea una nueva tienda (Location es opcional)
        Task<StoreInformationResponseDto?> CreateAsync(StoreInformationDto dto);
        
        // Actualiza una tienda existente (Location puede ser null)
        Task<StoreInformationResponseDto> UpdateAsync(int id, StoreInformationDto dto);
        
        // Elimina una tienda (solo si no tiene StoreLinks asociados)
        Task DeleteAsync(int id);
    }
}