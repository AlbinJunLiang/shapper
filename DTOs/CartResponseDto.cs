namespace Shapper.Dtos
{
    public class CartResponseDto
    {
        public List<CartItemValidationDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal Total { get; set; }
        public bool AllItemsAvailable { get; set; }
    }
}
