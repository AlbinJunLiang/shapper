namespace Shapper.Dtos
{
    public class AuthUserResultDto
    {
        public string UserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Role { get; set; }
        public bool EmailVerified { get; set; } = false;
    }
}
