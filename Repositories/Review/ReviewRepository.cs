using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.Reviews
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<Review?> GetByIdAsync(int id) => await _context.Reviews.FindAsync(id);

        public async Task<(List<Review> Reviews, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Reviews.AsNoTracking();

            var totalCount = await query.CountAsync();

            var reviews = await query
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (reviews, totalCount);
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
