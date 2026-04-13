namespace Shapper.Dtos.Users;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public int RoleId { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Status { get; set; }
}
