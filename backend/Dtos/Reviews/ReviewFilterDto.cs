// Dtos/ReviewFilterDto.cs
namespace Shapper.Dtos.Reviews
{
    public class ReviewFilterDto
    {
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public string? SortBy { get; set; }
    }
}
