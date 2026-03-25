using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos;
using Shapper.Enums;
using Shapper.Mappings;
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
                .Products.Include(p => p.Subcategory) // Carga la entidad Subcategory
                .Include(p => p.ProductImages) // Carga la lista de Images
                .FirstOrDefaultAsync(p => p.Id == id); // FindAsync no permite Includes, usamos FirstOrDefaultAsync
        }

        public async Task<Product?> GetByNameAsync(string name)
        {
            return await _context.Products.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<(List<ProductResponseDto> Products, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Products.AsNoTracking();

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name) // siempre ordena al paginar
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    Discount = p.Discount,
                    TaxAmount = p.TaxAmount,
                    Details = p.Details,
                    Status = p.Status,
                    SubcategoryId = p.SubcategoryId,
                    Images = p
                        .ProductImages.Select(i => new ProductImageDto { ImageUrl = i.ImageUrl })
                        .ToList(),
                })
                .ToListAsync();

            return (products, totalCount);
        }

        /**
        * @ ProductStoreViewDto
        Endpoint de acceso público para la tienda
        */
        public async Task<(List<ProductStoreViewDto>, int)> GetProductsStoreViewAsync(
            int page,
            int pageSize,
            bool onlyFeatured = false
        )
        {
            var baseQuery = _context
                .Products.AsNoTracking()
                .Where(p => p.Status == ProductStatus.ACTIVE.ToString());
            if (onlyFeatured)
                baseQuery = baseQuery.Where(p => p.FeaturedProduct != null);

            var totalCount = await baseQuery.CountAsync();

            var products = await baseQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MappingProduct.ToStoreViewDto)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<List<ProductStoreViewDto>> SearchProductsAsync(
            string searchTerm,
            int count = 5
        )
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ProductStoreViewDto>();

            var terms = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var query = _context
                .Products.AsNoTracking()
                .Where(p => p.Status == ProductStatus.ACTIVE.ToString());

            foreach (var term in terms)
            {
                query = query.Where(p => p.Name.Contains(term) || p.Description.Contains(term));
            }

            return await query
                .OrderBy(p => p.Name)
                .Take(count)
                .Select(MappingProduct.ToStoreViewDto)
                .ToListAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<(
            List<ProductStoreViewDto> Products,
            int TotalCount
        )> GetFilteredProductsAsync(ProductFilterDto filter, int page, int pageSize)
        {
            var query = _context
                .Products.AsNoTracking()
                .Where(p => p.Status == ProductStatus.ACTIVE.ToString());

            if (filter.SubcategoryIds != null && filter.SubcategoryIds.Any())
            {
                query = query.Where(p => filter.SubcategoryIds.Contains(p.SubcategoryId));
            }
            else if (filter.CategoryIds != null && filter.CategoryIds.Any())
            {
                query = query.Where(p => filter.CategoryIds.Contains(p.Subcategory.CategoryId));
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            var totalCount = await query.CountAsync();

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(MappingProduct.ToStoreViewDto)
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
