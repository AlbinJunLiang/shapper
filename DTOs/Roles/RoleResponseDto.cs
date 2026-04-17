namespace Shapper.Dtos.Roles
{
    public class RoleResponseDto
    {
        public int Id { get; set; }

        // CS8618 Fixed: Name cannot be null
        public string Name { get; set; } = string.Empty;

        // Al tener el '?', el compilador permite que sea nulo, 
        // así que este no genera advertencia.
        public string? Description { get; set; }
    }
}