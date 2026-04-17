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
            await _context.StoreInformations.AddAsync(storeInformation);
            await _context.SaveChangesAsync();
        }

        public async Task<StoreInformation?> GetByIdAsync(int id)
        {
            return await _context
                .StoreInformations.Include(s => s.StoreLinks)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StoreInformation?> GetByNameAsync(string name)
        {
            return await _context.StoreInformations.FirstOrDefaultAsync(s =>
                s.Name.ToLower() == name.ToLower()
            );
        }

        public async Task<StoreInformation?> GetByEmailAsync(string email)
        {
            return await _context.StoreInformations.FirstOrDefaultAsync(s =>
                s.Email.ToLower() == email.ToLower()
            );
        }

        public async Task<(
            List<StoreInformation> StoreInformations,
            int TotalCount
        )> GetPaginatedAsync(int page, int pageSize)
        {
            var query = _context.StoreInformations.Include(s => s.StoreLinks).AsNoTracking();

            var totalCount = await query.CountAsync();

            var storeInformations = await query
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (storeInformations, totalCount);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            var query = _context.StoreInformations.Where(s => s.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.StoreInformations.Where(s => s.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return !await query.AnyAsync();
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
