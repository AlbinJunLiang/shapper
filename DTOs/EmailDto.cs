namespace Shapper.Dtos
{
    public class EmailDto
    {
        public string To { get; init; } = null!;
        public string Subject { get; init; } = null!;
        public string HtmlContent { get; init; } = null!;
        public string SenderName { get; init; } = null!;
        public string SenderEmail { get; init; } = null!;
    }
}
