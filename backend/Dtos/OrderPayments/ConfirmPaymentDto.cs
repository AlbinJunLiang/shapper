using Shapper.Dtos.Orders;

namespace Shapper.Dtos.OrderPayments
{
    public class ConfirmPaymentDto
    {
        // Usamos null! porque este objeto DEBE venir del proceso de pago
        public OrderResponseDto Order { get; set; } = null!;

        // Inicializamos con string.Empty para las referencias de pago
        public string PaidId { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
    }
}
