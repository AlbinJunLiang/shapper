using Shapper.Dtos;
using Shapper.Dtos.Reviews;

namespace Shapper.Services.Reviews
{
    public interface IReviewService
    {
        Task<ReviewDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<ReviewResponseDto>> GetPaginatedAsync(int page, int pageSize);
        Task<ReviewFilterResponseDto> GetFilteredReviewsAsync(
            ReviewFilterDto filter,
            int page,
            int pageSize
        );
        Task<ReviewResponseDto?> CreateAsync(ReviewDto dto);

        Task<ReviewResponseDto> UpdateAsync(int id, ReviewDto dto, string email); // ← Agregar email

        Task DeleteAsync(int id, string email); // ← Recibe email, no userId
    }
}
