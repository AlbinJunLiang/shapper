using Microsoft.EntityFrameworkCore;
using Shapper.Data;
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
            await _context.Faqs.AddAsync(faq);
            await _context.SaveChangesAsync();
        }

        public async Task<Faq?> GetByIdAsync(int id)
        {
            return await _context.Faqs
                .Include(f => f.Store)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<List<Faq>> GetByStoreIdAsync(int storeId)
        {
            return await _context.Faqs
                .Where(f => f.StoreId == storeId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<(List<Faq> Faqs, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeId = null)
        {
            var query = _context.Faqs
                .Include(f => f.Store)
                .AsNoTracking();

            if (storeId.HasValue)
                query = query.Where(f => f.StoreId == storeId.Value);

            var totalCount = await query.CountAsync();

            var faqs = await query
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (faqs, totalCount);
        }

        public async Task<bool> ExistsByStoreAndQuestionAsync(int storeId, string question, int? excludeId = null)
        {
            var query = _context.Faqs.Where(f =>
                f.StoreId == storeId &&
                f.Question.ToLower() == question.ToLower());

            if (excludeId.HasValue)
                query = query.Where(f => f.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<Faq> UpdateAsync(Faq faq)
        {
            faq.UpdatedAt = DateTime.UtcNow;
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