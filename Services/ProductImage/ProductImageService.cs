using AutoMapper;
using Shapper.Dtos.ProductImages;
using Shapper.Models;
using Shapper.Repositories.ProductImages;
using Shapper.Repositories.Products;
using Shapper.Services.ImageStorage;

namespace Shapper.Services.ProductImages
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ImageStrategyFactory _imageFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductImageService> _logger;


        public ProductImageService(
            IProductImageRepository productImageRepository,
            IProductRepository productRepository,
            ImageStrategyFactory imageFactory,
            IMapper mapper,
            ILogger<ProductImageService> logger)
        {
            _productImageRepository = productImageRepository;
            _productRepository = productRepository;
            _imageFactory = imageFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductImageResponseDto> CreateAsync(CreateProductImageDto dto)
        {
            // Validar que el producto existe
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {dto.ProductId} does not exist.");

            string imageUrl;
            string resourceReference = string.Empty;

            // Opción 1: Subir archivo
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                var strategy = _imageFactory.Create(dto.Provider);
                var (path, publicId) = await strategy.UploadImageAsync(dto.ImageFile);

                imageUrl = path.StartsWith("http")
                    ? path
                    : $"/{path}";  // Ruta local
                resourceReference = publicId;
            }
            // Opción 2: Usar URL proporcionada
            else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                imageUrl = dto.ImageUrl;
                resourceReference = dto.ImageUrl; // Usar la URL como referencia
            }
            else
            {
                throw new ArgumentException("Either ImageFile or ImageUrl must be provided.");
            }

            var productImage = new ProductImage
            {
                ProductId = dto.ProductId,
                ImageUrl = imageUrl,
                ResourceReference = resourceReference
            };

            var result = await _productImageRepository.AddAsync(productImage);
            return _mapper.Map<ProductImageResponseDto>(result);
        }

        public async Task<ProductImageResponseDto?> GetByIdAsync(int id)
        {
            var image = await _productImageRepository.GetByIdAsync(id);
            return image == null ? null : _mapper.Map<ProductImageResponseDto>(image);
        }

        public async Task<List<ProductImageResponseDto>> GetByProductIdAsync(int productId)
        {
            var images = await _productImageRepository.GetByProductIdAsync(productId);
            return _mapper.Map<List<ProductImageResponseDto>>(images);
        }


        public async Task<ProductImageResponseDto> UpdateAsync(int id, UpdateProductImageDto dto)
        {
            var existingImage = await _productImageRepository.GetByIdAsync(id);
            if (existingImage == null)
                throw new KeyNotFoundException($"ProductImage with ID {id} not found.");

            // Caso 1: Se subió un nuevo archivo de imagen
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                // Eliminar imagen anterior si existe
                if (!string.IsNullOrEmpty(existingImage.ResourceReference))
                {
                    try
                    {
                        var deleteStrategy = _imageFactory.Create(dto.Provider);
                        await deleteStrategy.DeleteImageAsync(existingImage.ResourceReference);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error deleting old image with reference {ResourceReference}", existingImage.ResourceReference);
                    }
                }

                var uploadStrategy = _imageFactory.Create(dto.Provider);
                var (path, publicId) = await uploadStrategy.UploadImageAsync(dto.ImageFile);

                existingImage.ImageUrl = path.StartsWith("http") ? path : $"/{path}";
                existingImage.ResourceReference = publicId;
            }

            // Caso 2: Solo se actualizó la URL (sin archivo)
            else if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                // Solo actualizar la URL, no tocar el archivo físico
                existingImage.ImageUrl = dto.ImageUrl;
                existingImage.ResourceReference = dto.ImageUrl;
            }
            // Caso 3: No se proporcionó ni archivo ni URL
            else
            {
                throw new ArgumentException("Either ImageFile or ImageUrl must be provided for update.");
            }

            var result = await _productImageRepository.UpdateAsync(existingImage);
            return _mapper.Map<ProductImageResponseDto>(result);
        }
        public async Task<bool> DeleteAsync(int id, string provider)
        {
            var image = await _productImageRepository.GetByIdAsync(id);
            if (image == null)
                return false;

            // Eliminar el archivo físico si existe
            if (!string.IsNullOrEmpty(image.ResourceReference))
            {
                try
                {
                    var strategy = _imageFactory.Create(provider);
                    await strategy.DeleteImageAsync(image.ResourceReference);
                }
                catch (Exception ex)
                {

                    _logger.LogWarning(ex,
                        "Error deleting physical image. ImageId: {Id}, Resource: {Resource}, Provider: {Provider}",
                        id, image.ResourceReference, provider);
                }
            }

            await _productImageRepository.DeleteAsync(image);
            return true;
        }

    }
}