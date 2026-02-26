using Shapper.Dtos;

namespace Shapper.Services.Verifications
{
    public interface IVerificationStrategy
    {
        Task<AuthUserResultDto> VerifyTokenAsync(string token);
    }
}
