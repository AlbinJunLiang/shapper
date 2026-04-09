// Dtos/ReviewFilterDto.cs
namespace Shapper.Dtos
{
    public class ReviewFilterDto
    {
        public int ProductId { get; set; }
        public int? UserId { get; set; }
        public string? SortBy { get; set; }
    }
}
