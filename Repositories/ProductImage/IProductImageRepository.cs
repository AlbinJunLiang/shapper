using Shapper.Models;

namespace Shapper.Repositories.ProductImages
{
    public interface IProductImageRepository
    {
        Task<ProductImage?> GetByIdAsync(int id);
        Task<List<ProductImage>> GetByProductIdAsync(int productId);
        Task<ProductImage> AddAsync(ProductImage productImage);
        Task<ProductImage> UpdateAsync(ProductImage productImage);
        Task DeleteAsync(ProductImage productImage);
        Task DeleteByProductIdAsync(int productId);
        Task<bool> ExistsAsync(int id);
        Task<int> CountByProductIdAsync(int productId);
        Task SaveChangesAsync();
    }
}