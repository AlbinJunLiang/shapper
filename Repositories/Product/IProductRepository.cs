using Shapper.Dtos.Products;
using Shapper.Models;

namespace Shapper.Repositories.Products
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByNameAsync(string name);

        Task<(List<Product> Products, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<(List<Product> Products, int TotalCount)> GetProductsStoreViewAsync(
            int page,
            int pageSize,
            bool onlyFeatured
        );
        Task<List<Product>> SearchProductsAsync(string searchTerm, int count = 5);
        Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(
            ProductFilterDto filter,
            int page,
            int pageSize
        );
        Task AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
