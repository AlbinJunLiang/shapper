using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos;
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

        public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);

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

        public async Task<(
            List<ProductStoreViewDto> Products,
            int TotalCount
        )> GetProductsStoreViewAsync(int page, int pageSize)
        {
            var baseQuery = _context.Products.AsNoTracking().Where(p => p.Status == "ACTIVE");

            var totalCount = await baseQuery.CountAsync();

            var products = await baseQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductStoreViewDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Discount = p.Discount,
                    TaxAmount = p.TaxAmount,
                    NewPrice = (p.Price * (1 - p.Discount / 100)) * (1 + p.TaxAmount / 100), // $(P \cdot (1 - D)) \cdot (1 + T)$
                    Quantity = p.Quantity,
                    Details = p.Details,
                    Status = p.Status,
                    SubcategoryName = p.Subcategory.Name,
                    Images = p
                        .ProductImages.Select(i => new ProductImageDto { ImageUrl = i.ImageUrl })
                        .ToList(),
                })
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
