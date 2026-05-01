using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Shapper.Services.Verifications;

namespace Shapper.Authentication
{
    public class ProviderAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly VerificationStrategyFactory _factory;

        public ProviderAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            VerificationStrategyFactory factory
        )
            : base(options, logger, encoder)
        {
            _factory = factory;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // 1. Verificación segura del Header
            if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                return AuthenticateResult.NoResult();

            string? headerValue = authorizationHeader.ToString();

            if (
                string.IsNullOrWhiteSpace(headerValue)
                || !headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
                return AuthenticateResult.NoResult();

            var token = headerValue.Substring("Bearer ".Length).Trim();

            try
            {
                var strategy = _factory.Create();
                var user = await strategy.VerifyTokenAsync(token);

                // CS8602 Prevention: Si user es null, fallamos la autenticación
                if (user == null)
                    return AuthenticateResult.Fail("Invalid user data from token.");

                var claims = new List<Claim>
                {
                    // Usamos el operador null-coalescing (??) para asegurar que no viajen nulos
                    new Claim(ClaimTypes.NameIdentifier, user.UserId ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
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
                return AuthenticateResult.Fail($"Authentication failed: {ex.Message}");
            }
        }
    }
}
