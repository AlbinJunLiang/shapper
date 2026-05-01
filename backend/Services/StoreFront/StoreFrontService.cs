
using Shapper.Dtos.StoreFront;
using Shapper.Services.Categories;
using Shapper.Services.Products;
using Shapper.Services.Stores;

namespace Shapper.Services.StoreFront
{
    public class StoreFrontService : IStoreFrontService
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreService _storeService;

        public StoreFrontService(
            IProductService productService,
            ICategoryService categoryService,
            IStoreService storeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _storeService = storeService;
        }

        public async Task<StoreHomeDataDto> GetHomeDataAsync(
            string storeCode,
            int productsPage = 1,
            int productsPageSize = 10,
            bool featured = true,
            int categoriesPage = 1,
            int categoriesPageSize = 8)
        {
            // Ejecutar en serie (evita errores de concurrencia)
            var storeInfo = await _storeService.GetByStoreCodeAsync(storeCode);
            var products = await _productService.GetProductsStoreViewAsync(productsPage, productsPageSize, featured);
            var categories = await _categoryService.GetPaginatedAsync(categoriesPage, categoriesPageSize);
            var priceRange = await _categoryService.GetCategoriesWithGlobalPriceRangeAsync();

            return new StoreHomeDataDto
            {
                StoreInfo = storeInfo,
                Products = products,
                Categories = categories,
                CategoriesWithPriceRange = priceRange,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}