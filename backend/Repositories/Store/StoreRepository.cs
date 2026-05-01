using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.Stores
{
    public class StoreRepository : IStoreRepository
    {
        private readonly AppDbContext _context;

        public StoreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Store store)
        {
            await _context.Stores.AddAsync(store);
            await _context.SaveChangesAsync();
        }

        public async Task<Store?> GetByIdAsync(int id)
        {
            return await _context
                .Stores.Include(s => s.StoreLinks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Stores.AnyAsync(s => s.Id == id);
        }

        public async Task<Store?> GetByStoreCodeAsync(string storeCode)
        {
            return await _context
                .Stores.Include(s => s.StoreLinks)
                .FirstOrDefaultAsync(s => s.StoreCode.Equals(storeCode));
        }

        public async Task<Store?> GetByNameAsync(string name)
        {
            return await _context.Stores.FirstOrDefaultAsync(s =>
                s.Name.ToLower() == name.ToLower()
            );
        }

        public async Task<Store?> GetByEmailAsync(string email)
        {
            return await _context.Stores.FirstOrDefaultAsync(s =>
                s.Email.ToLower() == email.ToLower()
            );
        }

        public async Task<(List<Store> stores, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Stores.Include(s => s.StoreLinks).AsNoTracking();

            var totalCount = await query.CountAsync();

            var stores = await query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (stores, totalCount);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.Stores.Where(s => s.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Stores.Where(s => s.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<Store> UpdateAsync(Store store)
        {
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
            return store;
        }

        public async Task DeleteAsync(Store store)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
        }
    }
}
