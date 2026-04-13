namespace Shapper.Dtos.Orders
{
    public class OrderDto
    {
        public string OrderReference { get; set; }
        public double Total { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; }
        public string? ExtraData { get; set; } // JSON aquí
    }
}
