using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.OrderDetails
{
    public interface IOrderDetailRepository
    {
        Task<OrderDetail> GetByIdAsync(int id);
        Task<(List<OrderDetail> OrderDetails, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task AddAsync(OrderDetail orderDetail);
        Task<OrderDetail> UpdateAsync(OrderDetail orderDetail);
        Task DeleteAsync(OrderDetail orderDetail);
    }
}
