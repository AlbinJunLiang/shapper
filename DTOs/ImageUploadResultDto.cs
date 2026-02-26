namespace Shapper.Dtos
{
    public class ImageUploadResult
    {
        public string? Url { get; set; }
        public string? FileName { get; set; }
        public string PublicId { get; set; } = default!;
    }
}
