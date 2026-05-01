using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Models;

namespace Shapper.Repositories.FeaturedProducts
{
    public class FeaturedProductRepository : IFeaturedProductRepository
    {
        private readonly AppDbContext _context;

        public FeaturedProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FeaturedProduct featuredProduct)
        {
            await _context.FeaturedProducts.AddAsync(featuredProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<FeaturedProduct?> GetByIdAsync(int id)
        {
            return await _context.FeaturedProducts
                .Include(fp => fp.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(fp => fp.Id == id);
        }

        public async Task<FeaturedProduct?> GetByProductIdAsync(int productId)
        {
            return await _context.FeaturedProducts
                .Include(fp => fp.Product)
                .FirstOrDefaultAsync(fp => fp.ProductId == productId);
        }

        public async Task<List<FeaturedProduct>> GetAllAsync()
        {
            return await _context.FeaturedProducts
                .Include(fp => fp.Product)
                .ThenInclude(p => p.ProductImages)
                .OrderByDescending(fp => fp.Id)
                .ToListAsync();
        }

        public async Task<(List<FeaturedProduct> FeaturedProducts, int TotalCount)> GetPaginatedAsync(int page, int pageSize)
        {
            var query = _context.FeaturedProducts
                .Include(fp => fp.Product)
                .ThenInclude(p => p.ProductImages)
                .AsNoTracking();

            var totalCount = await query.CountAsync();

            var featuredProducts = await query
                .OrderByDescending(fp => fp.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (featuredProducts, totalCount);
        }

        public async Task<bool> ExistsByProductIdAsync(int productId)
        {
            return await _context.FeaturedProducts
                .AnyAsync(fp => fp.ProductId == productId);
        }

        public async Task<int> CountAsync()
        {
            return await _context.FeaturedProducts.CountAsync();
        }

        public async Task<FeaturedProduct> UpdateAsync(FeaturedProduct featuredProduct)
        {
            _context.FeaturedProducts.Update(featuredProduct);
            await _context.SaveChangesAsync();
            return featuredProduct;
        }

        public async Task DeleteAsync(FeaturedProduct featuredProduct)
        {
            _context.FeaturedProducts.Remove(featuredProduct);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var featured = await _context.FeaturedProducts
                .FirstOrDefaultAsync(fp => fp.ProductId == productId);

            if (featured != null)
            {
                _context.FeaturedProducts.Remove(featured);
                await _context.SaveChangesAsync();
            }
        }
    }
}