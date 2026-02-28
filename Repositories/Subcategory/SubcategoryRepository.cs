using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.Subcategories
{
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly AppDbContext _context;

        public SubcategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Subcategory subcategory)
        {
            _context.Subcategories.Add(subcategory);
            await _context.SaveChangesAsync();
        }

        public async Task<Subcategory?> GetByIdAsync(int id) =>
            await _context.Subcategories.FindAsync(id);

        public async Task<Subcategory?> GetByNameAsync(string name)
        {
            return await _context.Subcategories.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<(List<Subcategory> Subcategories, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Subcategories.AsNoTracking();

            var totalCount = await query.CountAsync();

            var subcategories = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (subcategories, totalCount);
        }

        public async Task<Subcategory> UpdateAsync(Subcategory subcategory)
        {
            _context.Subcategories.Update(subcategory);
            await _context.SaveChangesAsync();
            return subcategory;
        }

        public async Task DeleteAsync(Subcategory subcategory)
        {
            _context.Subcategories.Remove(subcategory);
            await _context.SaveChangesAsync();
        }
    }
}
