using Shapper.Services.Verifications.Strategies;

namespace Shapper.Services.Verifications
{
    public class VerificationStrategyFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public VerificationStrategyFactory(
            IConfiguration configuration,
            IServiceProvider serviceProvider
        )
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public IVerificationStrategy Create()
        {
            var provider = _configuration["Auth:Provider"]; // Asegúrate de tener esto en appsettings.json

            return provider switch
            {
                // Pedimos la instancia al contenedor para que traiga sus dependencias
                "Firebase" => _serviceProvider.GetRequiredService<FirebaseVerificationStrategy>(),
                "Supabase" => _serviceProvider.GetRequiredService<SupabaseVerificationStrategy>(),
                _ => throw new Exception(
                    "Proveedor de autenticación no válido configurado en appsettings."
                ),
            };
        }

        public string GetProviderName()
        {
            return _configuration["Auth:Provider"] ?? "Default";
        }
    }
}
