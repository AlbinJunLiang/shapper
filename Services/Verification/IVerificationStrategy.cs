using Shapper.DTOs;

namespace Shapper.Services.Verifications
{
    public interface IVerificationStrategy
    {
        Task<AuthUserResultDto> VerifyTokenAsync(string token);
    }
}
