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
        private readonly IMapper _mapper;

        public FeaturedProductService(
            IFeaturedProductRepository featuredProductRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _featuredProductRepository = featuredProductRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<FeaturedProductResponseDto?> CreateAsync(FeaturedProductDto dto)
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

            return new FeaturedProductResponseDto
            {
                Id = featuredProduct.Id,
                ProductId = featuredProduct.ProductId
            };
        }

        public async Task<FeaturedProductResponseDto?> GetByIdAsync(int id)
        {
            var featuredProduct = await _featuredProductRepository.GetByIdAsync(id);
            if (featuredProduct == null)
                return null;

            return new FeaturedProductResponseDto
            {
                Id = featuredProduct.Id,
                ProductId = featuredProduct.ProductId
            };
        }

        public async Task<PagedResponseDto<FeaturedProductResponseDto>> GetPaginatedAsync(int page, int pageSize)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (featuredProducts, totalCount) = await _featuredProductRepository.GetPaginatedAsync(page, pageSize);

            var mapped = featuredProducts.Select(fp => new FeaturedProductResponseDto
            {
                Id = fp.Id,
                ProductId = fp.ProductId
            }).ToList();

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
                throw new InvalidOperationException("Featured product not found.");

            // Validar que el nuevo producto existe
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {dto.ProductId} does not exist.");

            existing.ProductId = dto.ProductId;
            await _featuredProductRepository.UpdateAsync(existing);

            return new FeaturedProductResponseDto
            {
                Id = existing.Id,
                ProductId = existing.ProductId
            };
        }

        public async Task DeleteAsync(int id)
        {
            var featuredProduct = await _featuredProductRepository.GetByIdAsync(id);
            if (featuredProduct == null)
                throw new InvalidOperationException("Featured product not found.");

            await _featuredProductRepository.DeleteAsync(featuredProduct);
        }
    }
}