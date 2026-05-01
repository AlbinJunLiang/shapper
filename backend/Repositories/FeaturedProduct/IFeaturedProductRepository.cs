using Shapper.Models;

namespace Shapper.Repositories.FeaturedProducts
{
    public interface IFeaturedProductRepository
    {
        Task<FeaturedProduct?> GetByIdAsync(int id);
        Task<FeaturedProduct?> GetByProductIdAsync(int productId);
        Task<List<FeaturedProduct>> GetAllAsync();
        Task<(List<FeaturedProduct> FeaturedProducts, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<bool> ExistsByProductIdAsync(int productId);
        Task<int> CountAsync();
        Task AddAsync(FeaturedProduct featuredProduct);
        Task<FeaturedProduct> UpdateAsync(FeaturedProduct featuredProduct);
        Task DeleteAsync(FeaturedProduct featuredProduct);
        Task DeleteByProductIdAsync(int productId);
    }
}