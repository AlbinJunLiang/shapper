using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.FeaturedProducts;
using Shapper.Models;
using Shapper.Repositories.FeaturedProducts;

namespace Shapper.Services.FeaturedProducts
{
    public class FeaturedProductService : IFeaturedProductService
    {
        private readonly IFeaturedProductRepository _featuredProductRepository;
        private readonly IMapper _mapper;

        public FeaturedProductService(
            IFeaturedProductRepository featuredProductRepository,
            IMapper mapper
        )
        {
            _featuredProductRepository = featuredProductRepository;
            _mapper = mapper;
        }

        public async Task<FeaturedProductResponseDto?> CreateAsync(FeaturedProductDto dto)
        {
            var category = _mapper.Map<FeaturedProduct>(dto);

            await _featuredProductRepository.AddAsync(category);

            return _mapper.Map<FeaturedProductResponseDto>(category);
        }

        public async Task<FeaturedProductDto?> GetByIdAsync(int id)
        {
            var category = await _featuredProductRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<FeaturedProductDto>(category);
        }

        public async Task<PagedResponseDto<FeaturedProductResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (FeaturedProducts, totalCount) = await _featuredProductRepository.GetPaginatedAsync(
                page,
                pageSize
            );

            var mapped = _mapper.Map<List<FeaturedProductResponseDto>>(FeaturedProducts);

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
            var existingOrder = await _featuredProductRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("FeaturedProduct not found.");
            _mapper.Map(dto, existingOrder);

            await _featuredProductRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<FeaturedProductResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _featuredProductRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("FeaturedProduct not found.");

            await _featuredProductRepository.DeleteAsync(category);
        }
    }
}
