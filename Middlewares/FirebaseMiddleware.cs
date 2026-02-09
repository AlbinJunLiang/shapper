using System.Security.Claims; // <-- necesario para Claim, ClaimTypes, ClaimsIdentity, ClaimsPrincipal
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;

namespace Shapper.Middlewares
{
    public class FirebaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly FirebaseService _firebaseService;

        public FirebaseAuthMiddleware(RequestDelegate next, FirebaseService firebaseService)
        {
            _next = next;
            _firebaseService = firebaseService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token no proporcionado");
                return;
            }

            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            try
            {
                var decodedToken = await _firebaseService.VerifyTokenAsync(token);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, decodedToken.Uid),
                };

                // Verificar si el token tiene correo y agregarlo
                if (decodedToken.Claims.ContainsKey("email"))
                {
                    claims.Add(
                        new Claim(ClaimTypes.Email, decodedToken.Claims["email"].ToString()!)
                    );
                }

                var identity = new ClaimsIdentity(claims, "Firebase");
                context.User = new ClaimsPrincipal(identity);

                await _next(context);
            }
            catch
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido");
            }
        }
    }
}
