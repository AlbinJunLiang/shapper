using Shapper.Models;

namespace Shapper.Repositories.Locations
{
    public interface ILocationRepository
    {
        Task<Location?> GetByIdAsync(int id);
        Task<(List<Location> Locations, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<double?> GetCostByIdAsync(int id);
        Task AddAsync(Location location);
        Task<Location> UpdateAsync(Location location);
        Task DeleteAsync(Location location);
    }
}
