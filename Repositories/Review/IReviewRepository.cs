using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.Reviews
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(int id); // Agregado ?
        Task<bool> ProductExistsAsync(int productId);
        Task<bool> UserExistsAsync(int userId);
        Task<bool> ReviewExistsAsync(int productId, int userId);

        // Actualizado para incluir RatingStats
        Task<(double AverageRating, int TotalReviews, List<RatingCountDto> RatingStats)> GetProductReviewStatsAsync(int productId);

        Task<(List<Review> Reviews, int TotalCount)> GetPaginatedAsync(int page, int pageSize);

        Task<(List<Review> Reviews, int TotalCount)> GetFilteredReviewsAsync(
            ReviewFilterDto filter,
            int page,
            int pageSize
        );

        Task AddAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(Review review);
    }
}