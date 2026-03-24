using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Products;
using Shapper.Repositories.Subcategories;

namespace Shapper.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ISubcategoryRepository _subcategoryRepository;

        private readonly IMapper _mapper;

        public ProductService(
            IProductRepository productRepository,
            ISubcategoryRepository subcategoryRepository,
            IMapper mapper
        )
        {
            _productRepository = productRepository;
            _subcategoryRepository = subcategoryRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponseDto> CreateAsync(ProductDto dto)
        {
            var existingSubcategory = await _subcategoryRepository.GetByIdAsync(dto.SubcategoryId);

            if (existingSubcategory == null)
                throw new InvalidOperationException("The specified subcategory does not exist.");

            var product = _mapper.Map<Product>(dto);

            await _productRepository.AddAsync(product);

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductStoreView2Dto?> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductStoreView2Dto>(product);
        }

        public async Task<PagedResponseDto<ProductResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (products, totalCount) = await _productRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<ProductResponseDto>>(products);

            return new PagedResponseDto<ProductResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<PagedResponseDto<ProductStoreViewDto>> GetProductsStoreViewAsync(
            int page,
            int pageSize,
            bool onlyFeatured
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (products, totalCount) = await _productRepository.GetProductsStoreViewAsync(
                page,
                pageSize,
                onlyFeatured
            );

            var mapped = _mapper.Map<List<ProductStoreViewDto>>(products);

            return new PagedResponseDto<ProductStoreViewDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<List<ProductStoreViewDto>> SearchProductsAsync(
            string searchTerm,
            int count = 5
        )
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm, count);
            return _mapper.Map<List<ProductStoreViewDto>>(products);
        }

        public async Task<PagedResponseDto<ProductStoreViewDto>> GetFilteredProductsAsync(
            ProductFilterDto filter,
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (products, totalCount) = await _productRepository.GetFilteredProductsAsync(
                filter,
                page,
                pageSize
            );

            return new PagedResponseDto<ProductStoreViewDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = products,
            };
        }

        public async Task<ProductResponseDto> UpdateAsync(int id, ProductDto dto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
                throw new InvalidOperationException("Product not found.");

            if (existingProduct.Id != id && existingProduct.Name == dto.Name)
                throw new InvalidOperationException("Product name already exists.");

            // Mapear dto → entidad existente
            _mapper.Map(dto, existingProduct);

            await _productRepository.UpdateAsync(existingProduct);

            // Mapear entidad → response
            return _mapper.Map<ProductResponseDto>(existingProduct);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                throw new InvalidOperationException("Product not found.");

            await _productRepository.DeleteAsync(product);
        }
    }
}
