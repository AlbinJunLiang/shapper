using Shapper.Dtos;
using Shapper.Models;

namespace Shapper.Repositories.Products
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetByNameAsync(string name);
        Task<(List<ProductResponseDto> Products, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        );

        Task<(List<ProductStoreViewDto> Products, int TotalCount)> GetProductsStoreViewAsync(
            int page,
            int pageSize
        );

        Task AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
