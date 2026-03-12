namespace Shapper.Dtos
{
    public class OrderDto
    {
        public string OrderReference { get; set; }
        public double Total { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
    }

    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderReference { get; set; }
        public double Total { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
