using AutoMapper;
using Shapper.Dtos;
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

        public async Task<SubcategoryResponseDto> CreateAsync(SubcategoryDto dto)
        {
            var existingSubcategory = await _subcategoryRepository.GetByNameAsync(dto.Name);
            var existingCategory = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            dto.Name = dto.Name?.Trim();

            if (existingCategory == null)
                throw new InvalidOperationException("The specified category does not exist.");

            var otherSubcategory = await _subcategoryRepository.GetByNameAsync(dto.Name);
            if (otherSubcategory != null)
                throw new InvalidOperationException("Subcategory name already exists.");

            var subcategory = _mapper.Map<Subcategory>(dto);

            await _subcategoryRepository.AddAsync(subcategory);

            return _mapper.Map<SubcategoryResponseDto>(subcategory);
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
            dto.Name = dto.Name?.Trim();

            if (existingSubcategory == null)
                throw new InvalidOperationException("Subcategory not found.");

            var otherSubcategory = await _subcategoryRepository.GetByNameAsync(dto.Name);
            if (otherSubcategory != null)
                throw new InvalidOperationException("Subcategory name already exists.");

            existingSubcategory.Name = dto.Name;
            existingSubcategory.Description = dto.Description;
            existingSubcategory.ImageUrl = dto.ImageUrl;

            await _subcategoryRepository.UpdateAsync(existingSubcategory);

            return new SubcategoryResponseDto
            {
                Id = existingSubcategory.Id,
                Name = existingSubcategory.Name,
                Description = existingSubcategory.Description,
                ImageUrl = existingSubcategory.ImageUrl,
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
