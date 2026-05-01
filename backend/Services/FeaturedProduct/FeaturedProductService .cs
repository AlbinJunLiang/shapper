using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.FeaturedProducts;
using Shapper.Models;
using Shapper.Repositories.FeaturedProducts;
using Shapper.Repositories.Products;

namespace Shapper.Services.FeaturedProducts
{
    public class FeaturedProductService : IFeaturedProductService
    {
        private readonly IFeaturedProductRepository _featuredProductRepository;
        private readonly IProductRepository _productRepository;
        public FeaturedProductService(
            IFeaturedProductRepository featuredProductRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _featuredProductRepository = featuredProductRepository;
            _productRepository = productRepository;
        }

        public async Task<FeaturedProductResponseDto> CreateAsync(FeaturedProductDto dto)
        {
            // Validar que el producto existe
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {dto.ProductId} does not exist.");

            var featuredProduct = new FeaturedProduct
            {
                ProductId = dto.ProductId
            };

            await _featuredProductRepository.AddAsync(featuredProduct);

            return MapToResponse(featuredProduct, product);
        }

        public async Task<FeaturedProductResponseDto?> GetByIdAsync(int id)
        {
            var featuredProduct = await _featuredProductRepository.GetByIdAsync(id);
            if (featuredProduct == null)
                return null;

            return MapToResponse(featuredProduct, featuredProduct.Product);
        }

        public async Task<List<FeaturedProductResponseDto>> GetAllAsync()
        {
            var featuredProducts = await _featuredProductRepository.GetAllAsync();
            var responses = new List<FeaturedProductResponseDto>();

            foreach (var fp in featuredProducts)
            {
                responses.Add(MapToResponse(fp, fp.Product));
            }

            return responses;
        }

        public async Task<PagedResponseDto<FeaturedProductResponseDto>> GetPaginatedAsync(int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (featuredProducts, totalCount) = await _featuredProductRepository.GetPaginatedAsync(page, pageSize);

            var mapped = new List<FeaturedProductResponseDto>();
            foreach (var fp in featuredProducts)
            {
                mapped.Add(MapToResponse(fp, fp.Product));
            }

            return new PagedResponseDto<FeaturedProductResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<FeaturedProductResponseDto> UpdateAsync(int id, FeaturedProductDto dto)
        {
            var existing = await _featuredProductRepository.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException($"Featured product with ID {id} not found.");

            // Validar que el nuevo producto existe
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {dto.ProductId} does not exist.");

            existing.ProductId = dto.ProductId;
            await _featuredProductRepository.UpdateAsync(existing);

            return MapToResponse(existing, product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var featuredProduct = await _featuredProductRepository.GetByIdAsync(id);
            if (featuredProduct == null)
                return false;

            await _featuredProductRepository.DeleteAsync(featuredProduct);
            return true;
        }

        public async Task<bool> DeleteByProductIdAsync(int productId)
        {
            if (!await _featuredProductRepository.ExistsByProductIdAsync(productId))
                return false;

            await _featuredProductRepository.DeleteByProductIdAsync(productId);
            return true;
        }

        public async Task<bool> IsProductFeaturedAsync(int productId)
        {
            return await _featuredProductRepository.ExistsByProductIdAsync(productId);
        }

        public async Task<int> GetFeaturedCountAsync()
        {
            return await _featuredProductRepository.CountAsync();
        }

        // Método privado para mapear a respuesta
        private FeaturedProductResponseDto MapToResponse(FeaturedProduct featured, Product? product)
        {
            return new FeaturedProductResponseDto
            {
                Id = featured.Id,
                ProductId = featured.ProductId,
                ProductName = product?.Name ?? string.Empty,
                ProductImageUrl = product?.ProductImages?.FirstOrDefault()?.ImageUrl,
                ProductPrice = product?.Price ?? 0,
            };
        }
    }
}