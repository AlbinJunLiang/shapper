using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos.Products;
using Shapper.Enums;
using Shapper.Models;

namespace Shapper.Repositories.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context
                .Products.Include(p => p.Subcategory)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByNameAsync(string name)
        {
            return await _context.Products.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Dictionary<int, Product>> GetProductsByIdsAsync(List<int> productIds)
        {
            return await _context
                .Products.Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);
        }

        public async Task<(List<Product> Products, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Products.Include(p => p.ProductImages).AsNoTracking();

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<(List<Product> Products, int TotalCount)> GetProductsStoreViewAsync(
            int page,
            int pageSize,
            bool onlyFeatured = false
        )
        {
            var baseQuery = _context
                .Products.Include(p => p.Subcategory)
                .Include(p => p.ProductImages)
                .Where(p => p.Status.ToUpper() == ProductStatus.ACTIVE.ToString());

            if (onlyFeatured)
                baseQuery = baseQuery.Where(p => p.FeaturedProduct != null);

            var totalCount = await baseQuery.CountAsync();

            var products = await baseQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm, int count = 5)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Product>();

            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var query = _context
                .Products.Include(p => p.Subcategory)
                .Include(p => p.ProductImages)
                .Where(p => p.Status.ToUpper() == ProductStatus.ACTIVE.ToString());

            foreach (var term in terms)
            {
                query = query.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
            }

            return await query.OrderBy(p => p.Name).Take(count).ToListAsync();
        }

        public async Task<(List<Product> Products, int TotalCount)> GetFilteredProductsAsync(
            ProductFilterDto filter,
            int page,
            int pageSize
        )
        {
            var query = _context
                .Products.Include(p => p.Subcategory)
                .Include(p => p.ProductImages)
                .Where(p => p.Status.ToUpper() == ProductStatus.ACTIVE.ToString());

            if (filter.SubcategoryIds != null && filter.SubcategoryIds.Any())
            {
                query = query.Where(p => filter.SubcategoryIds.Contains(p.SubcategoryId));
            }

            if (filter.CategoryIds != null && filter.CategoryIds.Any())
            {
                query = query.Where(p => filter.CategoryIds.Contains(p.Subcategory.CategoryId));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
