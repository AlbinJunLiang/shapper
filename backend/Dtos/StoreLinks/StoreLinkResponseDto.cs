namespace Shapper.Dtos.StoreLinks
{
    public class StoreLinkResponseDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
