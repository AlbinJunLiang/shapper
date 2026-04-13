using Shapper.Dtos;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Models;

namespace Shapper.Services.OrderPayments
{
    public interface IOrderPaymentService
    {
        Task<OrderPaymentDto?> GetByIdAsync(int id);

        Task<OrderPaymentDto> GetByTransactionReferenceAsync(string transactionReference);

        Task<PagedResponseDto<OrderPaymentResponseDto>> GetPaginatedAsync(int page, int pageSize);

        Task<bool> CreateAsync(ConfirmPaymentDto orderData);

        Task<OrderPaymentResponseDto> UpdateAsync(int id, OrderPaymentDto dto);

        Task DeleteAsync(int id);
    }
}
