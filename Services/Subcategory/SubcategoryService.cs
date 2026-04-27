using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Subcategories;
using Shapper.Models;
using Shapper.Repositories.Categories;
using Shapper.Repositories.Subcategories;
using Shapper.Services.ImageStorage;
using Shapper.Services.ProductImages;

namespace Shapper.Services.Subcategories
{
    public class SubcategoryService : ISubcategoryService
    {
        private readonly ISubcategoryRepository _subcategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ImageStrategyFactory _imageFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductImageService> _logger;


        public SubcategoryService(
            ISubcategoryRepository subcategoryRepository,
            ICategoryRepository categoryRepository,
            ImageStrategyFactory imageFactory,
            IMapper mapper,
            ILogger<ProductImageService> logger)
        {
            _subcategoryRepository = subcategoryRepository;
            _categoryRepository = categoryRepository;
            _imageFactory = imageFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SubcategoryResponseDto> CreateAsync(SubcategoryDto dto)
        {
            // Validar nombre
            string cleanName = (dto.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(cleanName))
                throw new InvalidOperationException("Subcategory name is required.");

            // Validar que la categoría existe
            var existingCategory = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (existingCategory == null)
                throw new InvalidOperationException("The specified category does not exist.");

            // Validar nombre único
            var existingSubcategory = await _subcategoryRepository.GetByNameAsync(cleanName);
            if (existingSubcategory != null)
                throw new InvalidOperationException("Subcategory name already exists.");

            var subcategory = _mapper.Map<Subcategory>(dto);
            subcategory.Name = cleanName;
            subcategory.ImageProvider = dto.ImageProvider ?? "";

            // Procesar imagen si viene
            await ProcessImageAsync(subcategory, dto);

            await _subcategoryRepository.AddAsync(subcategory);

            return _mapper.Map<SubcategoryResponseDto>(subcategory);
        }

        public async Task<SubcategoryResponseDto?> GetByIdAsync(int id)
        {
            var subcategory = await _subcategoryRepository.GetByIdAsync(id);
            return subcategory == null ? null : _mapper.Map<SubcategoryResponseDto>(subcategory);
        }

        public async Task<PagedResponseDto<SubcategoryResponseDto>> GetPaginatedAsync(int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (subcategories, totalCount) = await _subcategoryRepository.GetPaginatedAsync(page, pageSize);
            var mapped = _mapper.Map<List<SubcategoryResponseDto>>(subcategories);

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
            if (existingSubcategory == null)
                throw new InvalidOperationException("Subcategory not found.");

            string cleanName = (dto.Name ?? string.Empty).Trim();

            // Validar nombre único si cambió
            if (existingSubcategory.Name != cleanName)
            {
                var otherSubcategory = await _subcategoryRepository.GetByNameAsync(cleanName);
                if (otherSubcategory != null && otherSubcategory.Id != id)
                    throw new InvalidOperationException("Subcategory name already exists.");
                existingSubcategory.Name = cleanName;
            }

            // Actualizar campos básicos
            existingSubcategory.Description = dto.Description ?? string.Empty;
            existingSubcategory.CategoryId = dto.CategoryId;
            existingSubcategory.ImageProvider = dto.ImageProvider ?? string.Empty;

            // Procesar imagen (puede ser archivo nuevo, URL nueva, o ninguna)
            await ProcessImageForUpdateAsync(existingSubcategory, dto);

            await _subcategoryRepository.UpdateAsync(existingSubcategory);

            return _mapper.Map<SubcategoryResponseDto>(existingSubcategory);
        }

        public async Task DeleteAsync(int id, string? provider = null)
        {
            var subcategory = await _subcategoryRepository.GetByIdAsync(id);
            if (subcategory == null)
                throw new InvalidOperationException("Subcategory not found.");

            // Pasas el provider aquí 👇
            await DeletePhysicalImageAsync(subcategory, provider);

            await _subcategoryRepository.DeleteAsync(subcategory);
        }
        #region Métodos Privados para Imagenes

        private async Task ProcessImageAsync(Subcategory subcategory, SubcategoryDto dto)
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

                subcategory.ImageUrl = path.StartsWith("http") ? path : $"/{path}";
                subcategory.ImageId = publicId;
                subcategory.ImageProvider = dto.ImageProvider;

            }
            else if (hasUrl)
            {
                // Solo guardar URL
                subcategory.ImageUrl = dto.ImageUrl!;
                subcategory.ImageId = dto.ImageUrl;
            }
        }

        private async Task ProcessImageForUpdateAsync(Subcategory subcategory, SubcategoryDto dto)
        {
            bool hasFile = dto.ImageFile != null && dto.ImageFile.Length > 0;
            bool hasUrl = !string.IsNullOrWhiteSpace(dto.ImageUrl);
            bool hasNewImage = hasFile || hasUrl;

            if (!hasNewImage)
                return; // No hay cambio de imagen

            // Si viene nueva imagen, eliminar la anterior
            if (!string.IsNullOrEmpty(subcategory.ImageId))
            {
                await DeletePhysicalImageAsync(subcategory, dto.ImageProvider);
            }

            if (hasFile)
            {
                var strategy = _imageFactory.Create(dto.ImageProvider);
                var (path, publicId) = await strategy.UploadImageAsync(dto.ImageFile!);

                subcategory.ImageUrl = path.StartsWith("http") ? path : $"/{path}";
                subcategory.ImageId = publicId;
                subcategory.ImageProvider = dto.ImageProvider;

            }
            else if (hasUrl)
            {
                subcategory.ImageUrl = dto.ImageUrl!;
                subcategory.ImageId = dto.ImageUrl;
            }
        }

        private async Task DeletePhysicalImageAsync(Subcategory subcategory, string? provider = null)
        {
            if (string.IsNullOrEmpty(subcategory.ImageId))
                return;

            var finalProvider = !string.IsNullOrEmpty(provider)
                ? provider
                : "local"; // fallback si no tienes nada mejor

            try
            {
                var strategy = _imageFactory.Create(finalProvider);
                await strategy.DeleteImageAsync(subcategory.ImageId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Error deleting image for subcategory {SubcategoryId}, Provider: {Provider}",
                    subcategory.Id, finalProvider);
            }
        }

        #endregion
    }
}