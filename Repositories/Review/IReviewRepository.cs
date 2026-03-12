using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.Reviews
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(int id);
        Task<(List<Review>Reviews, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task AddAsync(Review review);
        Task<Review> UpdateAsync(Review review);
        Task DeleteAsync(Review review);
    }
}
