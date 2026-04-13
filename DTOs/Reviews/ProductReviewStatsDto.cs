namespace Shapper.Dtos.Reviews
{
    public class ProductReviewStatsDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<RatingCountDto> RatingStats { get; set; } = new();
    }
}
