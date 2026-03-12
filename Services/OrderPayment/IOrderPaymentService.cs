using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Services.OrderPayments
{
    public interface IOrderPaymentService
    {
        Task<OrderPaymentDto?> GetByIdAsync(int id);

        Task<PagedResponseDto<OrderPaymentResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<OrderPaymentResponseDto?> CreateAsync(OrderPaymentDto dto);

        Task<OrderPaymentResponseDto> UpdateAsync(int id, OrderPaymentDto dto);

        Task DeleteAsync(int id);
    }
}
