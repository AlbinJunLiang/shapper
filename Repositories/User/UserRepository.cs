using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

        public async Task<List<User?>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
