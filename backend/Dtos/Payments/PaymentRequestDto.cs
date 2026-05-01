using Shapper.Dtos.Orders;

namespace Shapper.Dtos.PaymentRequests;

public class PaymentRequestDto
{
    public string Provider { get; set; } = ""; // "paypal" o "stripe"
    public string SuccessUrl { get; set; } = "";
    public string CancelUrl { get; set; } = "";
    public OrderResponseDto OrderResponse { get; set; } = null!;
}
