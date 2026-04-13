using Shapper.Dtos.OrderDetails;

namespace Shapper.Dtos.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderReference { get; set; }
        public double Total { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalTax { get; set; }
        public double Subtotal { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; }
        public string? ExtraData { get; set; }
        public string CompanyName { get; set; }

        public DateTime CreatedAt { get; set; }
        public List<OrderDetailResponseDto> Details { get; set; } = new();
    }
}
