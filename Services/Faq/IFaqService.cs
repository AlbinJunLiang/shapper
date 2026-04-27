using Shapper.Dtos;
using Shapper.Dtos.Faqs;

namespace Shapper.Services.Faqs
{
    public interface IFaqService
    {
        Task<FaqResponseDto?> GetByIdAsync(int id);
        Task<List<FaqResponseDto>> GetByStoreIdAsync(int storeId);
        Task<PagedResponseDto<FaqResponseDto>> GetPaginatedAsync(int page, int pageSize, int? storeId = null);
        Task<FaqResponseDto> CreateAsync(FaqDto dto);
        Task<FaqResponseDto> UpdateAsync(int id, FaqUpdateDto dto);
        Task DeleteAsync(int id);
        Task<FaqResponseDto> ToggleStatusAsync(int id);
    }
}