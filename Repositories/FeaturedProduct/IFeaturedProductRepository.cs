using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.FeaturedProducts
{
    public interface IFeaturedProductRepository
    {
        Task<FeaturedProduct> GetByIdAsync(int id);
        Task<(List<FeaturedProduct> FeaturedProducts, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );
        Task AddAsync(FeaturedProduct featuredProduct);
        Task<FeaturedProduct> UpdateAsync(FeaturedProduct featuredProduct);
        Task DeleteAsync(FeaturedProduct featuredProduct);
    }
}
