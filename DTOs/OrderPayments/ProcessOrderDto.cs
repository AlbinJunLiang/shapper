using Shapper.Dtos.Orders;

namespace Shapper.Dtos.OrderPayments
{
    public class ProcessOrderDto
    {
        public string PaidId { get; set; }
        public string Reference { get; set; }
        public string PaymentMethod { get; set; }
    }
}
