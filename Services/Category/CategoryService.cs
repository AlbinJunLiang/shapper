using AutoMapper;
using Shapper.Dtos;
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

        public async Task<CategoryResponseDto> CreateAsync(CategoryDto dto)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(dto.Name);

            if (existingCategory != null)
                throw new InvalidOperationException("Category name already exists.");

            var category = _mapper.Map<Category>(dto);

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

        public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryDto dto)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);

            if (existingCategory == null)
                throw new InvalidOperationException("Category not found.");

            if (existingCategory.Name == dto.Name)
                throw new InvalidOperationException("Category name already exists.");

            existingCategory.Name = dto.Name;
            existingCategory.Description = dto.Description;
            existingCategory.ImageUrl = dto.ImageUrl;

            await _categoryRepository.UpdateAsync(existingCategory);

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
