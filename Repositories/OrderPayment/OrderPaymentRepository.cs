using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.OrderPayments
{
    public class OrderPaymentRepository : IOrderPaymentRepository
    {
        private readonly AppDbContext _context;

        public OrderPaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(OrderPayment orderPayment)
        {
            _context.OrderPayments.Add(orderPayment);
            return Task.CompletedTask; // Esto quita el warning y permite que el servicio use "await"
        }

        public async Task<OrderPayment?> GetByIdAsync(int id) =>
            await _context.OrderPayments.FindAsync(id);

        public async Task<OrderPayment?> GetByTransactionReferenceAsync(string transactionReference)
        {
            return await _context.OrderPayments.FirstOrDefaultAsync(p =>
                p.TransactionReference == transactionReference
            );
        }

        public async Task<(List<OrderPayment> OrderPayments, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.OrderPayments.AsNoTracking();

            var totalCount = await query.CountAsync();

            var orderPayments = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orderPayments, totalCount);
        }

        public async Task<OrderPayment> UpdateAsync(OrderPayment orderPayment)
        {
            _context.OrderPayments.Update(orderPayment);
            await _context.SaveChangesAsync();
            return orderPayment;
        }

        public async Task DeleteAsync(OrderPayment orderPayment)
        {
            _context.OrderPayments.Remove(orderPayment);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
