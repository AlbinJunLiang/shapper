using Shapper.Models;

namespace Shapper.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task<(List<User> Users, int TotalCount)> GetPaginatedAsync(int page, int pageSize);

        Task AddAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
