using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos.Faqs;
using Shapper.Models;

namespace Shapper.Repositories.Faqs
{
    public class FaqRepository : IFaqRepository
    {
        private readonly AppDbContext _context;

        public FaqRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Faq faq)
        {
            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();
        }

        public async Task<Faq?> GetByIdAsync(int id) => await _context.Faqs.FindAsync(id);

        public async Task<(List<Faq> Faqs, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Faqs.AsNoTracking();

            var totalCount = await query.CountAsync();

            var faqs = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (faqs, totalCount);
        }

        public async Task<Faq> UpdateAsync(Faq faq)
        {
            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task DeleteAsync(Faq faq)
        {
            _context.Faqs.Remove(faq);
            await _context.SaveChangesAsync();
        }
    }
}
