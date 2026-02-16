using Shapper.Services.ImageStorage.Strategies;

namespace Shapper.Services.ImageStorage
{
    public class ImageStrategyFactory
    {
        private readonly IServiceProvider _provider;

        public ImageStrategyFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IImageService Create()
        {
            var config = _provider.GetRequiredService<IConfiguration>();
            var type = config["ImageStorage"];

            return type switch
            {
                "Cloudinary" => _provider.GetRequiredService<CloudinaryImageStrategy>(),
                _ => _provider.GetRequiredService<LocalImageStrategy>(),
            };
        }
    }
}
