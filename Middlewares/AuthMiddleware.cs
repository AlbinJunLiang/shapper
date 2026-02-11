using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Shapper.Services.Verifications;

namespace Shapper.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Extraer el header
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token no proporcionado o formato inválido.");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            // 2. Obtener la fábrica desde el ServiceProvider del contexto
            var factory = context.RequestServices.GetRequiredService<VerificationStrategyFactory>();

            try
            {
                var strategy = factory.Create();
                var userDto = await strategy.VerifyTokenAsync(token);

                // 3. (RECOMENDADO) Llenar el ClaimsPrincipal para que funcione User.Identity.Name, etc.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userDto.UserId),
                    new Claim(ClaimTypes.Email, userDto.Email),
                };
                if (!string.IsNullOrEmpty(userDto.Role))
                    claims.Add(new Claim(ClaimTypes.Role, userDto.Role));
                var providerName = factory.GetProviderName(); // O leerlo de builder.Configuration["Auth:Provider"]
                var identity = new ClaimsIdentity(claims, providerName);
                Console.WriteLine(identity);
                context.User = new ClaimsPrincipal(identity);

                // También puedes mantener context.Items si lo prefieres
                context.Items["User"] = userDto;

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync($"Token inválido: {ex.Message}");
            }
        }
    }
}
