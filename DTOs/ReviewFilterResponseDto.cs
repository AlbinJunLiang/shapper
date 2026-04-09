namespace Shapper.Dtos
{
    public class ReviewFilterResponseDto
    {
        public PagedResponseDto<ReviewResponse2Dto> Reviews { get; set; }
        public ProductReviewStatsDto ProductStats { get; set; }
    }
}
