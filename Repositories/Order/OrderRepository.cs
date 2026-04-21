using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.Orders
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // En OrderRepository.cs
        public Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            return Task.CompletedTask; // Esto quita el warning y permite que el servicio use "await"
        }

        public async Task<Order?> GetByIdAsync(int id) => await _context.Orders.FindAsync(id);

        public async Task<Order?> GetByReferenceAsync(string reference)
        {
            return await _context
                .Orders.Include(o => o.OrderDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(o => o.OrderReference == reference);
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Orders.AsNoTracking();

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetUserOrdersAsync(
            int userId,
            int days,
            int page,
            int pageSize
        )
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            var query = _context
                .Orders.AsNoTracking()
                .Where(o => o.CustomerId == userId && o.CreatedAt >= fromDate);

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return Task.FromResult(order);
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return false;

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
