namespace Shapper.Dtos.Roles
{
    public class RoleDto
    {
        public string Name { get; set; } = string.Empty;

        // Al tener el '?', el compilador permite que sea nulo,
        // así que este no genera advertencia.
        public string? Description { get; set; }
    }
}
