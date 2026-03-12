
        using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.Locations
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        public async Task<Location?> GetByIdAsync(int id) =>
            await _context.Locations.FindAsync(id);

        public async Task<(List<Location> Locations, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Locations.AsNoTracking();

            var totalCount = await query.CountAsync();

            var locations = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (locations, totalCount);
        }

        public async Task<Location> UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task DeleteAsync(Location location)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }
    }
}

        