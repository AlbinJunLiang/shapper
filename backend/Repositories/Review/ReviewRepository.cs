using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos.Reviews;
using Shapper.Enums;
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

        public async Task<bool> ProductExistsAsync(int productId) =>
            await _context.Products.AnyAsync(p => p.Id == productId);

        public async Task<bool> UserExistsAsync(int userId) =>
            await _context.Users.AnyAsync(u => u.Id == userId);

        public async Task<bool> ReviewExistsAsync(int productId, int userId)
        {
            return await _context.Reviews.AnyAsync(r =>
                r.ProductId == productId
                && r.UserId == userId
                && r.Status == ReviewStatus.ACTIVE.ToString()
            ); // Agregamos el filtro de estado
        }

        public async Task<(
            double AverageRating,
            int TotalReviews,
            List<RatingCountDto> RatingStats
        )> GetProductReviewStatsAsync(int productId)
        {
            var query = _context.Reviews.Where(r =>
                r.ProductId == productId && r.Status == ReviewStatus.ACTIVE.ToString()
            );

            var totalReviews = await query.CountAsync();

            var averageRating =
                totalReviews > 0 ? Math.Round(await query.AverageAsync(r => r.Rating), 1) : 0;

            var ratingStats = await query
                .GroupBy(r => r.Rating)
                .Select(g => new RatingCountDto { Stars = g.Key, Count = g.Count() })
                .ToListAsync();

            return (averageRating, totalReviews, ratingStats);
        }

        public async Task<(List<Review> Reviews, int TotalCount)> GetFilteredReviewsAsync(
            ReviewFilterDto filter,
            int page,
            int pageSize
        )
        {
            var query = _context
                .Reviews.Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.Status == ReviewStatus.ACTIVE.ToString())
                .AsNoTracking();

            if (filter.ProductId > 0)
                query = query.Where(r => r.ProductId == filter.ProductId);

            if (filter.UserId.HasValue && filter.UserId > 0)
                query = query.Where(r => r.UserId == filter.UserId);

            query = filter.SortBy?.ToLower() switch
            {
                "top-rated" => query
                    .OrderByDescending(r => r.Rating)
                    .ThenByDescending(r => r.CreatedAt),
                "recent" => query.OrderByDescending(r => r.CreatedAt),
                _ => query.OrderByDescending(r => r.CreatedAt),
            };

            var totalCount = await query.CountAsync();
            var reviews = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

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
