using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Categories;
using Shapper.Models;
using Shapper.Repositories.Categories;
using Shapper.Services.ImageStorage;

namespace Shapper.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ImageStrategyFactory _imageFactory;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ImageStrategyFactory imageFactory,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _imageFactory = imageFactory;
            _mapper = mapper;
        }

        public async Task<CategoryResponseDto?> CreateAsync(CategoryDto dto)
        {
            string cleanName = dto.Name?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(cleanName))
                throw new InvalidOperationException("Category name is required.");

            var existingCategory = await _categoryRepository.GetByNameAsync(cleanName);
            if (existingCategory != null)
                throw new InvalidOperationException("Category name already exists.");

            var category = _mapper.Map<Category>(dto);
            category.Name = cleanName;
            category.ImageProvider = dto.ImageProvider ?? "LOCAL";


            // Procesar imagen si viene
            await ProcessImageAsync(category, dto);

            await _categoryRepository.AddAsync(category);

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<PagedResponseDto<CategoryResponseDto>> GetPaginatedAsync(int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (categories, totalCount) = await _categoryRepository.GetPaginatedAsync(page, pageSize);
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

            string cleanName = dto.Name?.Trim() ?? string.Empty;

            // Validar nombre único si cambió
            if (existingCategory.Name != cleanName)
            {
                var otherCategory = await _categoryRepository.GetByNameAsync(cleanName);
                if (otherCategory != null)
                    throw new InvalidOperationException("Category name already exists.");
                existingCategory.Name = cleanName;
            }

            // Actualizar campos básicos
            existingCategory.Description = dto.Description ?? string.Empty;
            existingCategory.ImageProvider = dto.ImageProvider ?? "LOCAL";

            // Procesar imagen (puede ser archivo nuevo, URL nueva, o ninguna)
            await ProcessImageForUpdateAsync(existingCategory, dto);

            await _categoryRepository.UpdateAsync(existingCategory);

            return new CategoryResponseDto
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name,
                Description = existingCategory.Description,
                ImageProvider = existingCategory.ImageProvider,
                ImageUrl = existingCategory.ImageUrl,
                ImageId = existingCategory.ImageId,
            };
        }

        public async Task DeleteAsync(int id, string provider)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new InvalidOperationException("Category not found.");

            // Eliminar imagen física si existe
            await DeletePhysicalImageAsync(category, provider);

            await _categoryRepository.DeleteAsync(category);
        }

        #region Métodos Privados para Imagenes

        private async Task ProcessImageAsync(Category category, CategoryDto dto)
        {
            bool hasFile = dto.ImageFile != null && dto.ImageFile.Length > 0;
            bool hasUrl = !string.IsNullOrWhiteSpace(dto.ImageUrl);

            if (!hasFile && !hasUrl)
                return; // Sin imagen, todo opcional

            if (hasFile)
            {
                // Subir archivo
                var strategy = _imageFactory.Create(dto.ImageProvider);
                var (path, publicId) = await strategy.UploadImageAsync(dto.ImageFile!);

                category.ImageUrl = path.StartsWith("http") ? path : $"/{path}";
                category.ImageId = publicId;
                category.ImageProvider = dto.ImageProvider;
            }
            else if (hasUrl)
            {
                // Solo guardar URL
                category.ImageUrl = dto.ImageUrl;
                category.ImageId = dto.ImageUrl;
            }
        }

        private async Task ProcessImageForUpdateAsync(Category category, CategoryDto dto)
        {
            bool hasFile = dto.ImageFile != null && dto.ImageFile.Length > 0;
            bool hasUrl = !string.IsNullOrWhiteSpace(dto.ImageUrl);
            bool hasNewImage = hasFile || hasUrl;

            if (!hasNewImage)
                return; // No hay cambio de imagen

            // Si viene nueva imagen, eliminar la anterior
            if (!string.IsNullOrEmpty(category.ImageId))
            {
                await DeletePhysicalImageAsync(category, dto.ImageProvider ?? "LOCAL");
            }

            if (hasFile)
            {
                var strategy = _imageFactory.Create(dto.ImageProvider);
                var (path, publicId) = await strategy.UploadImageAsync(dto.ImageFile!);

                category.ImageUrl = path.StartsWith("http") ? path : $"/{path}";
                category.ImageId = publicId;
                category.ImageProvider = dto.ImageProvider;
            }
            else if (hasUrl)
            {
                category.ImageUrl = dto.ImageUrl;
                category.ImageId = dto.ImageUrl;
            }
        }

        private async Task DeletePhysicalImageAsync(Category category, string provider)
        {
            if (string.IsNullOrEmpty(category.ImageId))
                return;

            try
            {
  
                var strategy = _imageFactory.Create(provider);
                await strategy.DeleteImageAsync(category.ImageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image for category {category.Id}: {ex.Message}");
                // No lanzamos excepción para no interrumpir la eliminación del registro
            }
        }

        #endregion
    }
}