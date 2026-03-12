namespace Shapper.Dtos
{
    public class OrderDetailDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Subtotal { get; set; }
    }

    public class OrderDetailResponseDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Subtotal { get; set; }
    }
}
