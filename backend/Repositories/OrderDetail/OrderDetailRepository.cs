using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.OrderDetails
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderDetail?> GetByIdAsync(int id) =>
            await _context.OrderDetails.FindAsync(id);

        public async Task<(List<OrderDetail> OrderDetails, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.OrderDetails.AsNoTracking();

            var totalCount = await query.CountAsync();

            var orderDetails = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orderDetails, totalCount);
        }

        public async Task<OrderDetail> UpdateAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
            return orderDetail;
        }

        public async Task DeleteAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }
    }
}
