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
            _context.FeaturedProducts.Add(featuredProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<FeaturedProduct?> GetByIdAsync(int id)
        {
            return await _context.FeaturedProducts.FindAsync(id);
        }

        public async Task<(List<FeaturedProduct> FeaturedProducts, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize)
        {
            var query = _context.FeaturedProducts.AsNoTracking();

            var totalCount = await query.CountAsync();

            var featuredProducts = await query
                .OrderBy(fp => fp.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (featuredProducts, totalCount);
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
    }
}