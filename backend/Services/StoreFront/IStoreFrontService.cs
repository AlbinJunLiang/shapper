using Shapper.Dtos.StoreFront;

namespace Shapper.Services.StoreFront
{
    public interface IStoreFrontService
    {
        Task<StoreHomeDataDto> GetHomeDataAsync(
            string storeCode,
            int productsPage = 1,
            int productsPageSize = 10,
            bool featured = true,
            int categoriesPage = 1,
            int categoriesPageSize = 8);
    }
}