using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.OrderDetails
{
    public interface IOrderDetailService
    {
        Task<OrderDetailDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<OrderDetailResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<OrderDetailResponseDto?> CreateAsync(OrderDetailDto dto);

        Task<OrderDetailResponseDto> UpdateAsync(int id, OrderDetailDto dto);

        Task DeleteAsync(int id);
    }
}
