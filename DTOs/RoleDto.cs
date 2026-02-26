namespace Shapper.Dtos
{
    public class RoleDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
