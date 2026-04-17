namespace Shapper.Dtos.Reviews
{
    public class ReviewFilterResponseDto
    {
        // CS8618 Fixed: Inicializado con una nueva instancia para evitar el warning
        public PagedResponseDto<ReviewResponse2Dto> Reviews { get; set; } = new();

        // CS8618 Fixed: Inicializado con una nueva instancia
        public ProductReviewStatsDto ProductStats { get; set; } = new();
    }
}