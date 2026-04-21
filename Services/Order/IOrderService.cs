using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.Orders;
using Shapper.Models;

namespace Shapper.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<OrderDto>> GetPaginatedAsync(int page, int pageSize);

        Task<PagedResponseDto<OrderDto>> GetUserOrdersAsync(
            int userId,
            int days,
            int page,
            int pageSize
        );

        Task<OrderResponseDto?> GetByReferenceAsync(string reference);

        Task<OrderResponseDto?> CreateAsync(CreateOrderDto dto);

        Task<OrderResponseDto> UpdateAsync(int id, OrderDto dto);

        Task<bool> UpdateStatusAsync(int orderId, string status);


        Task DeleteAsync(int id);
    }
}
