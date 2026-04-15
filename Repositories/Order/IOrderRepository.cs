using Shapper.Models;

namespace Shapper.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task<(List<Order> Orders, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<Order?> GetByReferenceAsync(string reference);

        Task AddAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task DeleteAsync(Order order);


        Task<(List<Order> Orders, int TotalCount)> GetUserOrdersAsync(
            int userId,
            int days,
            int page,
            int pageSize
        );

        Task SaveChangesAsync();
    }
}
