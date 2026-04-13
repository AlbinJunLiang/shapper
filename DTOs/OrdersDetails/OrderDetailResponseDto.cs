namespace Shapper.Dtos.OrderDetails
{
    public class OrderDetailResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; }
        public int RequestedQuantity { get; set; }
        public int? ActualQuantity { get; set; }
        public double BasePrice { get; set; } // precio original
        public double Discount { get; set; } // monto descontado
        public double Tax { get; set; } // monto de IVA
        public double FinalPrice { get; set; } // precio final unitario
        public double Subtotal { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
