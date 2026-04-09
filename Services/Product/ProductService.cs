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
        private readonly IMapper _mapper; // ✅ Mapper SOLO en el servicio

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

        public async Task<ProductStoreView2Dto?> GetByIdAsync(int id)
        {
            // 1. Obtener entidad del repositorio
            var product = await _productRepository.GetByIdAsync(id);

            // 2. Mapear a DTO en el servicio
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

            // 1. Obtener entidades del repositorio
            var (products, totalCount) = await _productRepository.GetPaginatedAsync(page, pageSize);

            // 2. Mapear a DTOs en el servicio
            var mappedProducts = _mapper.Map<List<ProductResponseDto>>(products);

            return new PagedResponseDto<ProductResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mappedProducts,
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

            // 1. Obtener entidades del repositorio
            var (products, totalCount) = await _productRepository.GetProductsStoreViewAsync(
                page,
                pageSize,
                onlyFeatured
            );

            // 2. Mapear a DTOs en el servicio
            var mappedProducts = _mapper.Map<List<ProductStoreViewDto>>(products);

            return new PagedResponseDto<ProductStoreViewDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mappedProducts,
            };
        }

        public async Task<List<ProductStoreViewDto>> SearchProductsAsync(
            string searchTerm,
            int count = 5
        )
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<ProductStoreViewDto>();

            // 1. Obtener entidades del repositorio
            var products = await _productRepository.SearchProductsAsync(searchTerm, count);

            // 2. Mapear a DTOs en el servicio
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

            // 1. Obtener entidades del repositorio
            var (products, totalCount) = await _productRepository.GetFilteredProductsAsync(
                filter,
                page,
                pageSize
            );

            // 2. Mapear a DTOs en el servicio
            var mappedProducts = _mapper.Map<List<ProductStoreViewDto>>(products);

            return new PagedResponseDto<ProductStoreViewDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mappedProducts,
            };
        }

        public async Task<ProductResponseDto?> CreateAsync(ProductDto dto)
        {
            var existingSubcategory = await _subcategoryRepository.GetByIdAsync(dto.SubcategoryId);
            if (existingSubcategory == null)
                throw new InvalidOperationException("The specified subcategory does not exist.");

            var existingProduct = await _productRepository.GetByNameAsync(dto.Name);
            if (existingProduct != null)
                throw new InvalidOperationException("A product with this name already exists.");

            // Mapear DTO a entidad
            var product = _mapper.Map<Product>(dto);
            await _productRepository.AddAsync(product);

            // Obtener la entidad creada con sus relaciones
            var createdProduct = await _productRepository.GetByIdAsync(product.Id);

            // Mapear entidad a DTO de respuesta
            return _mapper.Map<ProductResponseDto>(createdProduct);
        }

        public async Task<ProductResponseDto> UpdateAsync(int id, ProductDto dto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);

            if (existingProduct == null)
                throw new InvalidOperationException("Product not found.");

            var otherProduct = await _productRepository.GetByNameAsync(dto.Name);
            if (otherProduct != null && otherProduct.Id != id)
                throw new InvalidOperationException("Product name already exists.");

            // Mapear DTO a entidad existente
            _mapper.Map(dto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);

            // Mapear entidad actualizada a DTO de respuesta
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
