namespace Shapper.DTOs
{
    public class AuthUserResultDto
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Role { get; set; }
    }
}
