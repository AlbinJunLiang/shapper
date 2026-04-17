namespace Shapper.Dtos
{
    public class EmailDto
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string? SenderName { get; set; } = string.Empty;
        public string? SenderEmail { get; set; } = string.Empty;
    }
}
