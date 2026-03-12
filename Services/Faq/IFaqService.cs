using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Faqs
{
    public interface IFaqService
    {
        Task<FaqDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<FaqResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<FaqResponseDto?> CreateAsync(FaqDto dto);

        Task<FaqResponseDto> UpdateAsync(int id, FaqDto dto);

        Task DeleteAsync(int id);
    }
}
