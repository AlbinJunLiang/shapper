using System.Text.Json; // Librería principal
using System.Text.Json.Serialization; // Solo si necesitas atributos especiales
using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Enums;
using Shapper.Services.Emails;
using Shapper.Services.OrderPayments;
using Shapper.Services.Orders;
using Shapper.Templates;

namespace Shapper.Services.PaymentWebhooks
{
    public class PaymentWebhookService : IPaymentWebhookService
    {
        private readonly IOrderService _orderService;
        private readonly IOrderPaymentService _orderPaymentService;
        private readonly EmailService _emailService;

        private readonly IMapper _mapper;

        public PaymentWebhookService(
            IMapper mapper,
            IOrderService orderService,
            IOrderPaymentService orderPaymentService,
            EmailService emailService
        )
        {
            _mapper = mapper;
            _orderService = orderService;
            _orderPaymentService = orderPaymentService;
            _emailService = emailService;
        }

        public async Task<string> ProcessAsync(ProcessOrderDto dto)
        {
            var orderResponse = await _orderService.GetByReferenceAsync(dto.Reference);

            if (orderResponse == null)
                return OrderPaymentStatus.Failed.ToString();

            var tag = await _orderPaymentService.CreateAsync(
                new ConfirmPaymentDto
                {
                    PaidId = dto.PaidId,
                    PaymentMethod = dto.PaymentMethod,
                    Order = orderResponse,
                }
            );

            if (tag == OrderPaymentStatus.AlreadyPaid.ToString())
            {
                // 1. Deserializamos el string JSON que viene de la base de datos

                // 2. Generamos el HTML y enviamos el correo
                var htmlContent = ProductTableTemplate.GenerateTableHtml(orderResponse);

                var emailDto = new EmailDto
                {
                    To = orderResponse?.ExtraData?.Email ?? "",
                    Subject = "Order Confirmation",
                    HtmlContent = htmlContent,
                    SenderName = orderResponse?.CompanyName ?? "",
                };

                await _emailService.SendAsync("brevo", emailDto);
            }

            return tag;
        }

        public static EmailDto Create(
            string to,
            string subject,
            string htmlContent,
            string senderName,
            string senderEmail
        )
        {
            return new EmailDto
            {
                To = to,
                Subject = subject,
                HtmlContent = htmlContent,
                SenderName = senderName,
                SenderEmail = senderEmail,
            };
        }
    }
}
