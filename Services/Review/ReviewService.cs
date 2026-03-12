using AutoMapper;
using Shapper.Dtos;
using Shapper.Models;
using Shapper.Repositories.Reviews;

namespace Shapper.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ReviewResponseDto> CreateAsync(ReviewDto dto)
        {
            var category = _mapper.Map<Review>(dto);

            await _reviewRepository.AddAsync(category);

            return _mapper.Map<ReviewResponseDto>(category);
        }

        public async Task<ReviewDto?> GetByIdAsync(int id)
        {
            var category = await _reviewRepository.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<ReviewDto>(category);
        }

        public async Task<PagedResponseDto<ReviewResponseDto>> GetPaginatedAsync(
            int page,
            int pageSize
        )
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            pageSize = pageSize > 100 ? 100 : pageSize;

            var (Reviews, totalCount) = await _reviewRepository.GetPaginatedAsync(page, pageSize);

            var mapped = _mapper.Map<List<ReviewResponseDto>>(Reviews);

            return new PagedResponseDto<ReviewResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mapped,
            };
        }

        public async Task<ReviewResponseDto> UpdateAsync(int id, ReviewDto dto)
        {
            var existingOrder = await _reviewRepository.GetByIdAsync(id);

            if (existingOrder == null)
                throw new InvalidOperationException("Review not found.");
            _mapper.Map(dto, existingOrder);

            await _reviewRepository.UpdateAsync(existingOrder);
            // Mapear entidad → response
            return _mapper.Map<ReviewResponseDto>(existingOrder);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _reviewRepository.GetByIdAsync(id);

            if (category == null)
                throw new InvalidOperationException("Review not found.");

            await _reviewRepository.DeleteAsync(category);
        }
    }
}
