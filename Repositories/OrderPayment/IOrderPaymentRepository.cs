using Shapper.Dtos;
using Shapper.Models;


namespace Shapper.Repositories.OrderPayments
{
    public interface IOrderPaymentRepository
    {
        Task<OrderPayment> GetByIdAsync(int id);
        Task<OrderPayment> GetByTransactionReferenceAsync(string transactionReference);

        Task<(List<OrderPayment> OrderPayments, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task AddAsync(OrderPayment orderPayment);
        Task<OrderPayment> UpdateAsync(OrderPayment orderPayment);
        Task DeleteAsync(OrderPayment orderPayment);
        Task SaveChangesAsync();
    }
}
