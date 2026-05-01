using Shapper.Enums;
using Shapper.Services.ImageStorage.Strategies;

namespace Shapper.Services.ImageStorage
{
    public class ImageStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ImageStrategyFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        // Crear estrategia por nombre (string)
        public IImageService Create(string? provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                // Usar el proveedor por defecto de configuración
                provider = _configuration["ImageStorage:DefaultProvider"] ?? "Local";
            }

            return provider.ToLower() switch
            {
                "cloudinary" => _serviceProvider.GetRequiredService<CloudinaryImageStrategy>(),
                "local" => _serviceProvider.GetRequiredService<LocalImageStrategy>(),
                _ => throw new NotSupportedException($"Image storage provider '{provider}' is not supported.")
            };
        }

        // Crear estrategia por enum
        public IImageService Create(ImageStorageType type)
        {
            return type switch
            {
                ImageStorageType.Cloudinary => _serviceProvider.GetRequiredService<CloudinaryImageStrategy>(),
                ImageStorageType.Local => _serviceProvider.GetRequiredService<LocalImageStrategy>(),
                _ => throw new NotSupportedException($"Image storage provider '{type}' is not supported.")
            };
        }

        // Obtener lista de proveedores disponibles
        public List<string> GetAvailableProviders()
        {
            return new List<string> { "local", "cloudinary" };
        }
    }
}