using Shapper.Models;

namespace Shapper.Repositories.Roles
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(int id);
        Task<Role?> GetByNameAsync(string name);
        Task<(List<Role> Roles, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<bool> RoleIsUsedAsync(int id);
        Task<List<Role>> GetAllAsync();
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
    }
}
