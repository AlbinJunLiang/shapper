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

        public async Task<StoreLink?> GetByIdAsync(int id)
        {
            return await _context
                .StoreLinks.Include(sl => sl.StoreInformation)
                .FirstOrDefaultAsync(sl => sl.Id == id);
        }

        public async Task<StoreLink?> GetByNameAndStoreAsync(
            string name,
            int storeInformationId,
            int? excludeId = null
        )
        {
            var query = _context.StoreLinks.Where(sl =>
                sl.Name.ToLower() == name.ToLower() && sl.StoreInformationId == storeInformationId
            );

            if (excludeId.HasValue)
                query = query.Where(sl => sl.Id != excludeId.Value);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<StoreLink>> GetByStoreIdAsync(int storeInformationId)
        {
            return await _context
                .StoreLinks.Where(sl => sl.StoreInformationId == storeInformationId)
                .OrderBy(sl => sl.Name)
                .ToListAsync();
        }

        public async Task<(List<StoreLink> StoreLinks, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize,
            int? storeInformationId = null
        )
        {
            var query = _context.StoreLinks.Include(sl => sl.StoreInformation).AsNoTracking();

            if (storeInformationId.HasValue)
                query = query.Where(sl => sl.StoreInformationId == storeInformationId.Value);

            var totalCount = await query.CountAsync();

            var storeLinks = await query
                .OrderBy(sl => sl.StoreInformationId)
                .ThenBy(sl => sl.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (storeLinks, totalCount);
        }

        public async Task<bool> StoreInformationExistsAsync(int storeInformationId)
        {
            return await _context.StoreInformations.AnyAsync(si => si.Id == storeInformationId);
        }

        public async Task<int> CountLinksByStoreAsync(int storeInformationId)
        {
            return await _context.StoreLinks.CountAsync(sl =>
                sl.StoreInformationId == storeInformationId
            );
        }

        public async Task<StoreLink> UpdateAsync(StoreLink storeLink)
        {
            storeLink.UpdatedAt = DateTime.UtcNow;
            _context.StoreLinks.Update(storeLink);
            await _context.SaveChangesAsync();
            return storeLink;
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
