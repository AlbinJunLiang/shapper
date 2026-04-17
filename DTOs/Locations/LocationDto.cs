namespace Shapper.Dtos.Locations
{
    public class LocationDto
    {
        // Inicializamos con string.Empty para garantizar que nunca sean null
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public double Cost { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
