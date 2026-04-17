using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Categories;
using Shapper.Models;
using Shapper.Repositories.Categories;

namespace Shapper.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDto?> CreateAsync(CategoryDto dto)
        {
            string cleanName = dto.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanName))
                throw new InvalidOperationException("Category name is required.");

            // 2. Solo hacemos UNA consulta a la base de datos (antes tenías dos)
            var existingCategory = await _categoryRepository.GetByNameAsync(cleanName);
            if (existingCategory != null)
                throw new InvalidOperationException("Category name already exists.");

            // 3. Mapeo y creación
            var category = _mapper.Map<Category>(dto);
            category.Name = cleanName; // Aseguramos que el nombre en la DB esté limpio

            await _categoryRepository.AddAsync(category);

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        public async Task<PagedResponseDto<CategoryResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (categories, totalCount) = await _categoryRepository.GetPaginatedAsync(
                page,
                pageSize
            );

            var mapped = _mapper.Map<List<CategoryResponseDto>>(categories);

            return new PagedResponseDto<CategoryResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<CategoriesWithGlobalPriceRangeDto> GetCategoriesWithGlobalPriceRangeAsync()
        {
            return await _categoryRepository.GetCategoriesWithGlobalPriceRangeAsync();
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryDto dto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new InvalidOperationException("Category not found.");

            // 1. Limpiamos el nombre con seguridad
            string cleanName = dto.Name?.Trim() ?? string.Empty;

            // 2. CORRECCIÓN DE LÓGICA: Solo validar si el nombre cambió
            if (existingCategory.Name != cleanName)
            {
                var otherCategory = await _categoryRepository.GetByNameAsync(cleanName);
                if (otherCategory != null)
                    throw new InvalidOperationException("Category name already exists.");
            }

            // 3. Asignaciones seguras (Matando los warnings)
            existingCategory.Name = cleanName;
            existingCategory.Description = dto.Description ?? string.Empty;
            existingCategory.ImageUrl = dto.ImageUrl ?? string.Empty;

            await _categoryRepository.UpdateAsync(existingCategory);
            // await _categoryRepository.SaveChangesAsync(); // Asegúrate de llamar al Save si tu Repo no lo hace

            return new CategoryResponseDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                ImageUrl = existingCategory.ImageUrl,
            };
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("Category not found.");

            await _categoryRepository.DeleteAsync(category);
        }
    }
}
