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

        public async Task<Subcategory?> GetByIdAsync(int id)
        {
            return await _context.Subcategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Subcategory?> GetByNameAsync(string name)
        {
            return await _context.Subcategories
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<(List<Subcategory> Subcategories, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize)
        {
            var query = _context.Subcategories
                .Include(s => s.Category)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var subcategories = await query
                .OrderBy(s => s.Id)
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subcategories.AnyAsync(s => s.Id == id);
        }
    }
}