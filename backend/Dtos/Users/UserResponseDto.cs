namespace Shapper.Dtos.Users
{
    public class UserResponseDto
    {
        public int Id { get; set; }

        // CS8618 Fixed: Todos los strings inicializados con string.Empty
        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Status { get; set; } = "ACTIVE";
    }
}
