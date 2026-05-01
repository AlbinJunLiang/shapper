using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.Reviews;
using Shapper.Models;
using Shapper.Repositories.Reviews;
using Shapper.Repositories.Users;

namespace Shapper.Services.Reviews
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;


        public ReviewService(IReviewRepository reviewRepository, IMapper mapper, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<ReviewResponseDto?> CreateAsync(ReviewDto dto)
        {
            // Validate product
            if (!await _reviewRepository.ProductExistsAsync(dto.ProductId))
                throw new InvalidOperationException(
                    $"Product with ID {dto.ProductId} does not exist."
                );

            // Validate user
            if (!await _reviewRepository.UserExistsAsync(dto.UserId))
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            // Validate duplicate review
            if (await _reviewRepository.ReviewExistsAsync(dto.ProductId, dto.UserId))
                throw new InvalidOperationException("You have already reviewed this product.");

            // Mapping and setup
            var review = _mapper.Map<Review>(dto);
            review.CreatedAt = DateTime.UtcNow;
            review.Status = "ACTIVE";

            await _reviewRepository.AddAsync(review);

            return _mapper.Map<ReviewResponseDto>(review);
        }

        public async Task<ReviewDto?> GetByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
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

        public async Task<ReviewFilterResponseDto> GetFilteredReviewsAsync(
            ReviewFilterDto filter,
            int page,
            int pageSize
        )
        {
            // 1. CORRECCIÓN AQUÍ: Quitamos el "_" porque el repo solo devuelve 2 elementos
            var (reviews, totalCount) = await _reviewRepository.GetFilteredReviewsAsync(
                filter,
                page,
                pageSize
            );

            var mappedReviews = _mapper.Map<List<ReviewResponse2Dto>>(reviews);

            var pagedResult = new PagedResponseDto<ReviewResponse2Dto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Data = mappedReviews,
            };

            var statsDto = new ProductReviewStatsDto();

            if (filter.ProductId > 0)
            {
                // 2. Aquí SÍ recibes 3 cosas, porque GetProductReviewStatsAsync sí devuelve la tupla de 3
                var (average, total, ratingStats) =
                    await _reviewRepository.GetProductReviewStatsAsync(filter.ProductId);

                statsDto.AverageRating = average;
                statsDto.TotalReviews = total;
                statsDto.RatingStats = ratingStats;
            }

            return new ReviewFilterResponseDto { Reviews = pagedResult, ProductStats = statsDto };
        }

        public async Task<ReviewResponseDto> UpdateAsync(int id, ReviewDto dto, string email)
        {
            // 1. Buscar el usuario por email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException($"User with email {email} not found.");

            // 2. Obtener la review existente
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {id} not found.");

            // 3. Validar que el usuario es el dueño de la review
            if (review.UserId != user.Id)
                throw new UnauthorizedAccessException("You can only update your own reviews.");

            // 4. Validar rating (si viene en el dto)
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new InvalidOperationException("Rating must be between 1 and 5.");

            // 5. Actualizar solo los campos permitidos
            review.Rating = dto.Rating;
            review.Comment = dto.Comment ?? string.Empty;
            // No actualizar ProductId, UserId, CreatedAt

            // 6. Guardar cambios
            await _reviewRepository.UpdateAsync(review);

            // 7. Retornar el DTO actualizado
            return _mapper.Map<ReviewResponseDto>(review);
        }

        public async Task DeleteAsync(int id, string email)
        {
            // 1. Buscar el usuario por email en la base de datos
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new UnauthorizedAccessException($"User with email {email} not found in database.");

            // 2. Obtener la review por ID
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new InvalidOperationException($"Review with ID {id} not found.");

            // 3. Comparar: el dueño de la review debe ser el mismo
            if (review.UserId != user.Id)
                throw new UnauthorizedAccessException("You can only delete your own reviews.");

            // 4. Eliminar
            await _reviewRepository.DeleteAsync(review);
        }
    }
}
