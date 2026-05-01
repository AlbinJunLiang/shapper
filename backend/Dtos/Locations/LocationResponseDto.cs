namespace Shapper.Dtos.Locations
{
    public class LocationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public double Cost { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
