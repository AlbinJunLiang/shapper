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

        Task<Dictionary<int, Product>> GetProductsByIdsAsync(List<int> ids);
        Task SaveChangesAsync();
    }
}
