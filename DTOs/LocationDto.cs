namespace Shapper.Dtos
{
    public class LocationDto
    {
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class LocationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
