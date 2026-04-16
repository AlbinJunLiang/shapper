namespace Shapper.Dtos.Orders
{
    public class OrderDto
    {
        public string OrderReference { get; set; }
        public int? LocationId { get; set; }
        public double Total { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
