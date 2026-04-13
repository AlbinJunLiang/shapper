using AutoMapper;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Helpers;
using Shapper.Helpers;
using Shapper.Services.Emails;
using Shapper.Services.OrderPayments;
using Shapper.Services.Orders;
using Shapper.Templates;
using Shapper.Dtos;

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

        public async Task<bool> ProcessAsync(ProcessOrderDto dto)
        {
            var orderResponse = await _orderService.GetByReferenceAsync(dto.Reference);

            if (orderResponse == null)
            {
                return false;
            }

            var success = await _orderPaymentService.CreateAsync(
                new ConfirmPaymentDto
                {
                    PaidId = dto.PaidId,
                    PaymentMethod = dto.PaymentMethod,
                    Order = orderResponse,
                }
            );

            /*  if (success)
              {
                  await _emailService.SendAsync(provider, email);
              }
              Console.WriteLine(ProductTableTemplate.GenerateTableHtml(orderResponse.Details));
  */

            string address = JsonHelper.GetValue(orderResponse.ExtraData, "to");
Console.WriteLine("------------------------------------------- " + address);
            var emailDto = new EmailDto
            {
                To = "destinatario@gmail.com",
                Subject = "Prueba de correo",
                HtmlContent = "<h1>Hola</h1><p>Este es un correo de prueba</p>",
                SenderName = "Tu Nombre",
                SenderEmail = "tucorreo@gmail.com",
            };
            return success;
        }
    }
}
