using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.ProductImages
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductImage> AddAsync(ProductImage productImage)
        {
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return productImage;
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _context.ProductImages
                .Include(pi => pi.Product)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductImage> UpdateAsync(ProductImage productImage)
        {
            _context.ProductImages.Update(productImage);
            await _context.SaveChangesAsync();
            return productImage;
        }

        public async Task DeleteAsync(ProductImage productImage)
        {
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var images = await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            _context.ProductImages.RemoveRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ProductImages.AnyAsync(pi => pi.Id == id);
        }

        public async Task<int> CountByProductIdAsync(int productId)
        {
            return await _context.ProductImages.CountAsync(pi => pi.ProductId == productId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}