using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shapper.DTOs;

namespace Shapper.Services.Verifications.Strategies
{
    public class SupabaseVerificationStrategy : IVerificationStrategy
    {
        private readonly IConfiguration _configuration;

        public SupabaseVerificationStrategy(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<AuthUserResultDto> VerifyTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new SecurityTokenException("Token no proporcionado");

            var jwtSecret = _configuration["Supabase:JwtSecret"];
            if (string.IsNullOrEmpty(jwtSecret))
                throw new Exception("JWT_SECRET no configurado en appsettings.json");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;
            try
            {
                // Validación básica de firma
                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                    },
                    out validatedToken
                );

                var jwtToken = (JwtSecurityToken)validatedToken;

                // Validación manual de audiencia
                if (!jwtToken.Audiences.Contains("authenticated"))
                    throw new SecurityTokenException("Audience inválida");

                // Validación manual de expiración
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;
                if (expClaim != null)
                {
                    var expUnix = long.Parse(expClaim);
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                    if (expDate < DateTimeOffset.UtcNow)
                        throw new SecurityTokenExpiredException("Token expirado");
                }

                // Extraer datos del usuario
                var user = new AuthUserResultDto
                {
                    UserId = jwtToken.Claims.First(c => c.Type == "sub").Value,
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "",
                    Role =
                        jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value
                        ?? "authenticated",
                };

                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                // Puedes loguear ex.Message para depuración
                throw new SecurityTokenException("Token inválido", ex);
            }
        }
    }
}
