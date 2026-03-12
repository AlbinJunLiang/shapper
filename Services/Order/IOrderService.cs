using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<OrderResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<OrderResponseDto?> CreateAsync(OrderDto dto);

        Task<OrderResponseDto> UpdateAsync(int id, OrderDto dto);

        Task DeleteAsync(int id);
    }
}
