using AutoMapper;
using Shapper.Dtos.Checkouts;
using Shapper.Dtos.Orders;
using Shapper.Services.Orders;
using Shapper.Services.Payment;
using Shapper.Services.PaymentUrlValidators;

namespace Shapper.Services.Checkouts
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IOrderService _orderService;
        private readonly PaymentService _paymentService;
        private readonly IPaymentUrlValidator _paymentUrlValidators;

        public CheckoutService(
            IOrderService orderService,
            PaymentService paymentService,
            IPaymentUrlValidator paymentUrlValidators
        )
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _paymentUrlValidators = paymentUrlValidators;
        }

        public async Task<CheckoutResponseDto> ProcessAsync(CreateOrderDto dto)
        {
            // 1. Validaciones previas (Fail Fast)
            ValidatePaymentUrls(dto.SuccessUrl, dto.CancelUrl);

            // 2. Ejecución de lógica de negocio (Creación de la Orden)
            var order = await _orderService.CreateAsync(dto);

            if (order == null)
                throw new InvalidOperationException("Failed to create the order record.");

            if (order.Details == null || !order.Details.Any())
                throw new InvalidOperationException(
                    "No valid products found in the created order."
                );
            // 3. Generación de Pago
            var paymentUrl = await GeneratePaymentUrlAsync(dto, order);

            return new CheckoutResponseDto
            {
                OrderReference = order!.OrderReference,
                PaymentUrl = paymentUrl,
            };
        }

        private void ValidatePaymentUrls(string successUrl, string cancelUrl)
        {
            if (!_paymentUrlValidators.IsValidUrl(successUrl))
                throw new BadHttpRequestException(
                    $"Invalid Success URL: {_paymentUrlValidators.GetFullUrl(successUrl)}"
                );

            if (!_paymentUrlValidators.IsValidUrl(cancelUrl))
                throw new BadHttpRequestException(
                    $"Invalid Cancel URL: {_paymentUrlValidators.GetFullUrl(cancelUrl)}"
                );
        }

        private async Task<string> GeneratePaymentUrlAsync(
            CreateOrderDto dto,
            OrderResponseDto order
        )
        {
            var strategy = _paymentService.GetStrategy(dto.Provider);
            var payment = await strategy.CreatePaymentAsync(dto.SuccessUrl, dto.CancelUrl, order);

            if (string.IsNullOrEmpty(payment))
                throw new InvalidOperationException("Could not generate the payment gateway URL.");

            return payment;
        }
    }
}
