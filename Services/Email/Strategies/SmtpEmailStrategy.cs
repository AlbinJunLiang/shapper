using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Shapper.Config;
using Shapper.DTOs;

namespace Shapper.Services.Emails.Strategies
{
    public class SmtpEmailStrategy : IEmailStrategy
    {
        private readonly SmtpSettings _settings;

        public SmtpEmailStrategy(IOptions<SmtpSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendAsync(EmailDto message)
        {
            using var smtp = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass),
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.SmtpUser, _settings.SenderName),
                Subject = message.Subject,
                Body = message.HtmlContent,
                IsBodyHtml = true,
            };

            mail.To.Add(message.To);

            await smtp.SendMailAsync(mail);
        }
    }
}
