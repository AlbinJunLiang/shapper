using Shapper.DTOs;

namespace Shapper.Services.Emails
{
    public interface IEmailStrategy
    {
        Task SendAsync(EmailDto message);
    }
}
