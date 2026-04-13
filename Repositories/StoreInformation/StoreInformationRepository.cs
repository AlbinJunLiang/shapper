using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.StoreInformations
{
    public class StoreInformationRepository : IStoreInformationRepository
    {
        private readonly AppDbContext _context;

        public StoreInformationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StoreInformation storeInformation)
        {
            _context.StoreInformations.Add(storeInformation);
            await _context.SaveChangesAsync();
        }

        public async Task<StoreInformation?> GetByIdAsync(int id) =>
            await _context.StoreInformations.FindAsync(id);

        public async Task<(
            List<StoreInformation> StoreInformations,
            int TotalCount
        )> GetPaginatedAsync(int page, int pageSize)
        {
            var query = _context.StoreInformations.AsNoTracking();

            var totalCount = await query.CountAsync();

            var storeInformations = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (storeInformations, totalCount);
        }

        public async Task<StoreInformation> UpdateAsync(StoreInformation storeInformation)
        {
            _context.StoreInformations.Update(storeInformation);
            await _context.SaveChangesAsync();
            return storeInformation;
        }

        public async Task DeleteAsync(StoreInformation storeInformation)
        {
            _context.StoreInformations.Remove(storeInformation);
            await _context.SaveChangesAsync();
        }
    }
}
