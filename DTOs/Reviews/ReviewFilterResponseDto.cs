namespace Shapper.Dtos.Reviews
{
    public class ReviewFilterResponseDto
    {
        public PagedResponseDto<ReviewResponse2Dto> Reviews { get; set; }
        public ProductReviewStatsDto ProductStats { get; set; }
    }
}
