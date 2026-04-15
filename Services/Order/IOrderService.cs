using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.Orders;
using Shapper.Models;

namespace Shapper.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<OrderResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<PagedResponseDto<OrderResponseDto>> GetUserOrdersAsync(
            int userId,
            int days,
            int page,
            int pageSize
        );

        Task<OrderResponseDto?> GetByReferenceAsync(string reference);

        Task<OrderResponseDto?> CreateAsync(CreateOrderDto dto);

        Task<OrderResponseDto> UpdateAsync(int id, OrderDto dto);

        Task DeleteAsync(int id);
    }
}
