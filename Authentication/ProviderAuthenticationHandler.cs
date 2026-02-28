using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Shapper.Services.Verifications;

namespace Shapper.Authentication
{
    /// <summary>
    /// Handles the authentication process for incoming HTTP requests using a custom Bearer token strategy.
    ///
    /// This method:
    /// 1. Extracts the Authorization header from the request.
    /// 2. Validates the Bearer token using a verification strategy.
    /// 3. Builds a ClaimsPrincipal with user information (UserId, Email, Role, EmailVerified).
    /// 4. Returns an AuthenticationTicket if the token is valid.
    ///
    /// Returns:
    /// - AuthenticateResult.Success if the token is valid.
    /// - AuthenticateResult.Fail if validation fails.
    /// - AuthenticateResult.NoResult if no Authorization header is present.
    ///
    /// This handler integrates with ASP.NET Core authentication middleware
    /// and enables the use of [Authorize] attributes throughout the application.
    /// </summary>
    /*
    Intercepta requests HTTP protegidos
    
    Lee el header Authorization
    
    Valida el token
    
    Crea el ClaimsPrincipal
    
    Devuelve AuthenticateResult
    */
    public class ProviderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly VerificationStrategyFactory _factory;

        public ProviderAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            VerificationStrategyFactory factory
        )
            : base(options, logger, encoder, clock)
        {
            _factory = factory;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.NoResult();

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(token))
                return AuthenticateResult.NoResult();

            try
            {
                var strategy = _factory.Create();

                var user = await strategy.VerifyTokenAsync(token);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId),
                    new Claim(ClaimTypes.Email, user.Email ?? ""),
                    new Claim("role", user.Role ?? "authenticated"),
                    new Claim("email_verified", user.EmailVerified.ToString().ToLower()),
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }
    }
}
