using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.StoreLinks
{
    public class StoreLinkRepository : IStoreLinkRepository
    {
        private readonly AppDbContext _context;

        public StoreLinkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StoreLink storeLink)
        {
            await _context.StoreLinks.AddAsync(storeLink);
            await _context.SaveChangesAsync();
        }

        public async Task<StoreLink> UpdateAsync(StoreLink storeLink)
        {
            storeLink.UpdatedAt = DateTime.UtcNow;
            _context.StoreLinks.Update(storeLink);
            await _context.SaveChangesAsync();
            return storeLink;
        }

        public async Task<StoreLink> UpsertAsync(StoreLink storeLink)
        {
            storeLink.UpdatedAt = DateTime.UtcNow;

            // Si el ID no existe, lo marcamos como nuevo
            if (storeLink.Id == 0)
            {
                storeLink.CreatedAt = DateTime.UtcNow;
                await _context.StoreLinks.AddAsync(storeLink);
            }
            else
            {
                _context.StoreLinks.Update(storeLink);
            }

            await _context.SaveChangesAsync();
            return storeLink;
        }

        public async Task<StoreLink?> GetByIdAsync(int id)
        {
            return await _context
                .StoreLinks.Include(sl => sl.Store)
                .FirstOrDefaultAsync(sl => sl.Id == id);
        }

        public async Task<StoreLink?> GetByNameAndStoreAsync(
            string name,
            int storeId,
            int? excludeId = null
        )
        {
            return await _context
                .StoreLinks.Where(x =>
                    x.StoreId == storeId
                    && x.Name == name
                    && (!excludeId.HasValue || x.Id != excludeId.Value)
                )
                .FirstOrDefaultAsync();
        }

        public async Task<List<StoreLink>> GetByStoreIdAsync(int storeId)
        {
            return await _context
                .StoreLinks.Where(sl => sl.StoreId == storeId)
                .OrderBy(sl => sl.Name)
                .ToListAsync();
        }

        public async Task<(List<StoreLink> StoreLinks, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeId = null
        )
        {
            var query = _context.StoreLinks.Include(sl => sl.Store).AsNoTracking();

            if (storeId.HasValue)
                query = query.Where(sl => sl.StoreId == storeId.Value);

            var totalCount = await query.CountAsync();

            var storeLinks = await query
                .OrderBy(sl => sl.StoreId)
                .ThenBy(sl => sl.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (storeLinks, totalCount);
        }

        public async Task<bool> StoreExistsAsync(int storeId)
        {
            return await _context.Stores.AnyAsync(si => si.Id == storeId);
        }

        public async Task<int> CountLinksByStoreAsync(int storeId)
        {
            return await _context.StoreLinks.CountAsync(sl => sl.StoreId == storeId);
        }

        public async Task DeleteAsync(StoreLink storeLink)
        {
            _context.StoreLinks.Remove(storeLink);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
