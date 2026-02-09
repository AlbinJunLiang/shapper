using Shapper.DTOs;

namespace Shapper.Services
{
    public interface IEmailStrategy
    {
        Task SendAsync(EmailDto message);
    }
}
