using Shapper.Dtos.Orders;

namespace Shapper.Dtos.OrderPayments
{
    public class ConfirmPaymentDto
    {
        public OrderResponseDto Order { get; set; }
        public string PaidId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
