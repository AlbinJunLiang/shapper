namespace Shapper.Dtos.Subcategories
{
    public class SubcategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public string? ImageProvider { get; set; } = string.Empty;

        public string? ImageId { get; set; }
        public int CategoryId { get; set; }
    }
}