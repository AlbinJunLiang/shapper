using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Subcategories;
using Shapper.Models;
using Shapper.Repositories.Categories;
using Shapper.Repositories.Subcategories;

namespace Shapper.Services.Subcategories
{
    public class SubcategoryService : ISubcategoryService
    {
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        private readonly IMapper _mapper;

        public SubcategoryService(
            ISubcategoryRepository subcategoryRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper
        )
        {
            _subcategoryRepository = subcategoryRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<SubcategoryResponseDto?> CreateAsync(SubcategoryDto dto)
        {
            // 1. Limpiamos el nombre primero y aseguramos que no sea nulo (Mata CS8604)
            string cleanName = (dto.Name ?? string.Empty).Trim();

            // 2. Verificaciones en paralelo (Opcional, pero más limpio)
            var existingCategory = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (existingCategory == null)
                throw new InvalidOperationException("The specified category does not exist.");

            // 3. Usamos el nombre ya limpio para la búsqueda (Evita doble llamada al repo)
            var existingSubcategory = await _subcategoryRepository.GetByNameAsync(cleanName);
            if (existingSubcategory != null)
                throw new InvalidOperationException("Subcategory name already exists.");

            // 4. Mapeo y persistencia
            var subcategory = _mapper.Map<Subcategory>(dto);

            // Aseguramos que el nombre en la entidad sea el nombre limpio
            subcategory.Name = cleanName;

            await _subcategoryRepository.AddAsync(subcategory);

            // 5. Retorno seguro
            var response = _mapper.Map<SubcategoryResponseDto>(subcategory);

            // Si el mapper devolviera nulo por error de configuración, lanzamos excepción
            return response
                ?? throw new InvalidOperationException("Error mapping subcategory response.");
        }

        public async Task<SubcategoryDto?> GetByIdAsync(int id)
        {
            var subcategory = await _subcategoryRepository.GetByIdAsync(id);
            return subcategory == null ? null : _mapper.Map<SubcategoryDto>(subcategory);
        }

        public async Task<PagedResponseDto<SubcategoryResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (categories, totalCount) = await _subcategoryRepository.GetPaginatedAsync(
                page,
                pageSize
            );

            var mapped = _mapper.Map<List<SubcategoryResponseDto>>(categories);

            return new PagedResponseDto<SubcategoryResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<SubcategoryResponseDto> UpdateAsync(int id, SubcategoryDto dto)
        {
            var existingSubcategory = await _subcategoryRepository.GetByIdAsync(id);

            // 1. Evitamos el warning CS8602 al hacer Trim()
            // Si dto.Name es nulo por alguna razón, usamos string.Empty
            string cleanName = (dto.Name ?? string.Empty).Trim();

            if (existingSubcategory == null)
                throw new InvalidOperationException("Subcategory not found.");

            // 2. Validación de duplicados con el nombre ya limpio
            var otherSubcategory = await _subcategoryRepository.GetByNameAsync(cleanName);

            // Verificamos que si existe otra subcategoría, no sea la misma que estamos editando
            if (otherSubcategory != null && otherSubcategory.Id != id)
                throw new InvalidOperationException("Subcategory name already exists.");

            // 3. Asignación segura de valores
            existingSubcategory.Name = cleanName;
            existingSubcategory.Description = dto.Description ?? string.Empty;
            existingSubcategory.ImageUrl = dto.ImageUrl ?? string.Empty;

            await _subcategoryRepository.UpdateAsync(existingSubcategory);

            return new SubcategoryResponseDto
            {
                Id = existingSubcategory.Id,
                Name = existingSubcategory.Name ?? string.Empty,
                Description = existingSubcategory.Description ?? string.Empty,
                ImageUrl = existingSubcategory.ImageUrl ?? string.Empty,
                CategoryId = existingSubcategory.CategoryId,
            };
        }

        public async Task DeleteAsync(int id)
        {
            var subcategory = await _subcategoryRepository.GetByIdAsync(id);

            if (subcategory == null)
                throw new InvalidOperationException("Subcategory not found.");

            await _subcategoryRepository.DeleteAsync(subcategory);
        }
    }
}
