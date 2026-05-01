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

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Subcategories)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> GetByNameAsync(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var processedName = name.Trim().ToLower();
            return await _context.Categories.FirstOrDefaultAsync(c =>
                c.Name != null && c.Name.ToLower() == processedName);
        }

        public async Task<(List<Category> Categories, int TotalCount)> GetPaginatedAsync(int page, int pageSize)
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
            var categories = await _context
                .Categories.Select(c => new CategoryWithSubcategoriesDto
                {
                    Id = c.Id,
                    Name = c.Name ?? "",
                    Subcategories = c.Subcategories
                        .Where(s => s.Products.Any(p => p.Status == ProductStatus.ACTIVE.ToString()))
                        .Select(s => new SubcategoryResponse2Dto { Id = s.Id, Name = s.Name })
                        .ToList(),
                })
                .Where(c => c.Subcategories.Any())
                .ToListAsync();

            var priceStats = await _context
                .Products.Where(p => p.Status == ProductStatus.ACTIVE.ToString())
                .GroupBy(p => 1)
                .Select(g => new
                {
                    MinPrice = g.Min(p => (double?)p.Price),
                    MaxPrice = g.Max(p => (double?)p.Price),
                })
                .FirstOrDefaultAsync();

            return new CategoriesWithGlobalPriceRangeDto
            {
                GlobalMinPrice = priceStats?.MinPrice ?? 0,
                GlobalMaxPrice = priceStats?.MaxPrice ?? 0,
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

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}