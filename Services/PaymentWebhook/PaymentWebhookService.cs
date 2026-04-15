using AutoMapper;
using Shapper.Dtos;
using Shapper.Dtos.OrderDetails;
using Shapper.Dtos.OrderPayments;
using Shapper.Dtos.Orders;
using Shapper.Helpers;
using Shapper.Helpers;
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

            var extraDataDto = CreateExtraDataDto(orderResponse.ExtraData);
            var htmlContent = ProductTableTemplate.GenerateTableHtml(orderResponse, extraDataDto);
            var emailDto = new EmailDto
            {
                To = extraDataDto.Email,
                Subject = "Order Confirmation",
                HtmlContent = htmlContent,
                SenderName = orderResponse.CompanyName,
            };

            if (success)
            {
                await _emailService.SendAsync("brevo", emailDto);
            }

            return success;
        }

        public static ExtraDataDto CreateExtraDataDto(string extraData)
        {
            if (string.IsNullOrWhiteSpace(extraData))
                return new ExtraDataDto();

            return new ExtraDataDto
            {
                Address = JsonHelper.GetValue(extraData, "address"),
                Email = JsonHelper.GetValue(extraData, "email"),
                PhoneNumber = JsonHelper.GetValue(extraData, "phoneNumber"),
                LastName = JsonHelper.GetValue(extraData, "lastName"),
                Name = JsonHelper.GetValue(extraData, "name"),
                Place = JsonHelper.GetValue(extraData, "place"),
                PostalCode = JsonHelper.GetValue(extraData, "postalCode"),
            };
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
