using Shapper.Dtos.Checkouts;
using Shapper.Dtos.Orders;

namespace Shapper.Services.Checkouts
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> ProcessAsync(CreateOrderDto dto);
    }
}
