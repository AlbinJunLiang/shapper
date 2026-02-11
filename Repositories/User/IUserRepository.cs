using Shapper.Models;

namespace Shapper.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
    }
}
