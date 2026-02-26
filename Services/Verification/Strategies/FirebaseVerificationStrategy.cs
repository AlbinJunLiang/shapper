using FirebaseAdmin.Auth;
using Shapper.Dtos;

namespace Shapper.Services.Verifications.Strategies
{
    public class FirebaseVerificationStrategy : IVerificationStrategy
    {
        private readonly FirebaseService _firebaseService;

        public FirebaseVerificationStrategy(FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
        }

        public async Task<AuthUserResultDto> VerifyTokenAsync(string token)
        {
            var decodedToken = await _firebaseService.VerifyTokenAsync(token);

            return new AuthUserResultDto
            {
                UserId = decodedToken.Uid,

                Email = decodedToken.Claims.ContainsKey("email")
                    ? decodedToken.Claims["email"]?.ToString()!
                    : "",

                Role = decodedToken.Claims.ContainsKey("role")
                    ? decodedToken.Claims["role"]?.ToString()
                    : null,
                EmailVerified =
                    decodedToken.Claims.ContainsKey("email_verified")
                    && bool.TryParse(
                        decodedToken.Claims["email_verified"]?.ToString(),
                        out var isVerified
                    )
                    && isVerified,
            };
        }
    }
}
