using Microsoft.EntityFrameworkCore;
using Shapper.Data;
using Shapper.Dtos.Categories;
using Shapper.Dtos.Subcategories;
using Shapper.Enums;
using Shapper.Models;

namespace Shapper.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> GetByIdAsync(int id) =>
            await _context.Categories.FindAsync(id);

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<(List<Category> Categories, int TotalCount)> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            var query = _context.Categories.AsNoTracking();

            var totalCount = await query.CountAsync();

            var categories = await query
                .OrderBy(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (categories, totalCount);
        }

        public async Task<CategoriesWithGlobalPriceRangeDto> GetCategoriesWithGlobalPriceRangeAsync()
        {
            // 1. Obtener categorías con sus subcategorías (solo subcategorías que tengan al menos un producto activo)
            var categories = await _context
                .Categories.Select(c => new CategoryWithSubcategoriesDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Subcategories = c
                        .Subcategories.Where(s =>
                            s.Products.Any(p => p.Status == ProductStatus.ACTIVE.ToString())
                        )
                        .Select(s => new SubcategoryResponse2Dto { Id = s.Id, Name = s.Name })
                        .ToList(),
                })
                .Where(c => c.Subcategories.Any()) // omitir categorías sin subcategorías con productos
                .ToListAsync();

            // 2. Obtener precios globales (mín y máx de todos los productos activos)
            var priceStats = await _context
                .Products.Where(p => p.Status == ProductStatus.ACTIVE.ToString())
                .GroupBy(p => 1) // agrupa todos para aplicar agregaciones
                .Select(g => new
                {
                    MinPrice = g.Min(p => (double?)p.Price),
                    MaxPrice = g.Max(p => (double?)p.Price),
                })
                .FirstOrDefaultAsync();

            return new CategoriesWithGlobalPriceRangeDto
            {
                GlobalMinPrice = priceStats?.MinPrice,
                GlobalMaxPrice = priceStats?.MaxPrice,
                Categories = categories,
            };
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
