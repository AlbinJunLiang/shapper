namespace Shapper.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Type { get; set; }
        public double? Cost { get; set; }
        public string? Status { get; set; }
        public string? PostalCode { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
